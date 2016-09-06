using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SpaceChaseLib

{
    #region Structs

    #region UnchangableStructs

    public struct StudentInfo           // This structure is used to hold the students information
    {
        public string studentLastName;      // Student's last name
        public string studentFirstName;     // Student's first name
        public string studentIdNumber;      // Student's ID number
        public string studentCourse;        // Students's course code
    }


    public struct ScoutControl           // This structure hold the thruster and control values for the Scout Ship
    {
        public double ThrustForward;    // This is the forward thrust (-ve thrust is rearward)
        public double ThrustRight;      // This is sideways, or strafing thrust (+ve i to the right of the scout)
        public double ThrustCW;         // This is angular/rotational thrust (+ve is clockwise)
                                        // Maximum thrust values can the found in the SCParameters.TXT file
        public bool ShieldOn;           // This is set to true if you want the shields to be on
        public bool EnergyExtractorOn;  // This is set to true if you want the Energy Extractor to be operating
        public bool MinerOn;            // This is set to true if you want the Miner to be operating
    }


    public struct report                        // This structre is used in task one to report on the blackhole location
    {
        public bool complete;                   // When this is set to true the task is complete
        public double X;                        // This has to hold the X coord of the BlackHole
        public double Y;                        // This has to hold the Y coord of the BlackHole
    }

    public struct GameStatus            // This structure holds the game status
    {
        public int GameLevel;           // current game level (zero if a task is being performed or in menu)
        public int TaskLevel;           // current task (zero if a game is playing or in menu)
        public int iteration;           // the number of times this task/game has been repeated
        public double LevelTimeMS;      // time taken so far in the game level or task
        public double TotalTimeMS;      // total game time taken for all levels
    }


    public struct ScoutStatus           // This structure hold the lastest status of the scout ship
    {
        public double deltaTime;        // Time since this was last called
        public double deltaVelocityForward; // change in forward velocity (+ve forward)
        public double deltaVelocityRight;   // change in sideways velocity (+ve right)
        public double deltaVelocityAngularCW;   // change in rotational velocity (+ve is clockwise)
        public double currentVelocityForward;   // current forward velocity (+ve forward)
        public double currentVelocityRight;     // current sideways velocity (+ve right)
        public double currentVelocityAngularCW; // current rotational velocity (+ve is clockwise)
        public double averageVelocityForward;   // average forward velocity (+ve forward)
        public double averageVelocityRight;     // average sideways velocity (+ve right)
        public double averageVelocityAngularCW; // average rotational velocity (+ve is clockwise)
        public double shieldEnergy;             // current level of shield energy (0.0 to 1.0)
        public double hullIntegrity;            // current level of hull integrity (0.0 to 1.0)
        public bool isShieldOn;                 // True if shield is on (Note: if shield is on but shield energy
                                                //  is zero then the sheild is not effective)
        public bool isExtractorOn;              // True if the extractor is on.
        public bool isMinerOn;                  // True if the miner is on.
    }

    public struct SensorInfo                    // contains the information gathered from a sensor
    {
        public double range;                    // The range to the detected object
        public double angle;                    // The angle from the front of the scout ship to the object
        public ObjectType objectType;           // This is the type of object detected
        public int objectID;                    // This is a unique ID for this object in its type
    }

    // ObjectType is an enumeration of the different object types the ship's sensors can detect
    public enum ObjectType { Asteroid, BlackHole, Distortion, CombatDrone, Factory }

    #endregion


    public struct pose
    {
        public double X;
        public double Y;
        public double angle;
    }
    #endregion

    public class SCLib
    {
        //Unchangable Globals


        // "m" represents member variables
        // "g" represents global variables

        // TODO: Add your global variables here
        private ScoutControl gScoutControl = new ScoutControl();
        private GPS gGPS = new GPS();
        private ScoutStatus gScoutStatus = new ScoutStatus();
        private GameStatus gGameStatus = new GameStatus();
        public double TwoPI = Math.PI * 2;



        PID gRotationalPID = new PID();
        PID gXPID = new PID();   // holds the PID parameters for the Sideways motion PID
        PID gYPID = new PID();   // holds the PID parameters for the forwards/backwards motion PID

        pose[] mPath = new pose[20];                 // Used to hold a series of waypoints for the scount to follow
        int mPathIndex;                              // Used to hold the current waypoint

        report mReportObject = new report();

        

        pose mAvoidThrust = new pose();

        private StreamWriter sw; //This holds the file to log data to in CSV format


        #region InterfaceOutputs


        // Method      : StudentDetails
        // Input       : na
        // Output      : SD (type:StudentInfo)
        // Description :
        //      This method is called once when the program is first run 
        //      and is used to record the students details
        public StudentInfo StudentDetails()
        {
            StudentInfo SD = new StudentInfo();

            SD.studentLastName = "Joordens";    // Replace the string with your last name
            SD.studentFirstName = "Matthew";  // Replace the string with your first name
            SD.studentIdNumber = "007";    // Replace the string with your student number
            SD.studentCourse = "001";    // Replace the string with your course code

            return SD;
        }

        // Method      : ScreenMessage
        // input       : na
        // output      : strMessage (type: string)
        // description :
        //      This method is call 60 times per second.
        //      It allows you to write a message to the top left of the screen.
        //      You may like to use this for debugging.
        //      Leave the message blank when submutting the assignments
        // Example:
        //      strMessage = "Ship X : " + X.ToString() + "\nShip Y : " + Y.ToString();
        //      Assuming that X and Y are the ships coordinates,
        //      setting strMessage above will display the following in the top left corner;
        //          Ship X : -1834.679834
        //          Ship Y : 352.6738307
        //      The escape character "\n" seen before the "Ship Y : " will cause a new line to be added.
        public String ScreenMessage()
        {
            string strMessage;

            strMessage = "Ship X :" + gScoutPose.X.ToString() + "\nShip Y : " + gScoutPose.Y.ToString() + "\nShip A : " + gScoutPose.angle.ToString() + "\nBH X = " + mBlackHolePose.X.ToString() + "\nBH Y = " + mBlackHolePose.Y.ToString();

            return strMessage;
        }

        // Method      : ThrustersAndControl
        // Input       : na
        // Output      : t (type:ScoutControl)
        // Description :
        //      The method is called 60 times a second and allows you
        //      to control the Scout Ship's motion by providing three types of thrust.
        //      It also controls the shields, Exctractor and Miner.
        //      ThrustForward represents the forward and aft thrusters driving the 
        //      ship forward (+ve value) or backwards (-ve value).
        //      ThrustRight provides sideways thrust with +ve to the right of the ship.
        //      ThrustCW controls the rotation thrusters with +ve thrust moving the ship clockwise.
        //      The thrust values all have a maximum which can be found 
        //      in the SCParameters.txt file.
        public ScoutControl ThrustersAndControl()
        {
            return gScoutControl;
        }


        // Method      : Report
        // Input       : na
        // Output      : r (type: report)
        // Description :
        //      This method is call 60 time per second.
        //      It is used during task 1 which required you to find and report
        //      the location of the blackhole. Once the blackhole has been found,
        //      set the X and Y coords of the blackhole and set complete to true.
        //      It is also used in task 2 to report an asteroid. Set complete to true
        //      when an asteroid has been detected. For this X and Y are not required.
        public report Report()
        {
            report r = new report();    // set up a report structure to return

            r.complete = false;         // Set to true when the report is ready
            r.X = 0;                    // Set to the blackhole's X coord
            r.Y = 0;                    // Set to the blackhole's Y coord

            return mReportObject;
        }


        #endregion

        #region InterfaceInputs

        // Method      : InitialiseGame
        // Input       : na
        // Output      : na
        // Description :
        //      This method is called once when the program is first run 
        //      and can be use to initialise any variables if required
        public void InitialiseGame()
        {
        }



        // Method      : GameStatus
        // Input       : gs (type:GameStatus)
        // Output      : na
        // Description : 
        //      This method is called 60 times a second and provides
        //      details on the current gaming status.
        //      You may used this information as you wish.
        public void GameStatus(GameStatus gs)
        {
            gGameStatus = gs;
        }


        // Method     : ProvideScoutStatus
        // Input      : ss (type: ScoutStatus)
        // Output     : na
        // Dscription :
        //      This method is call 60 times per second and provides the status of the scout ship.
        //      You will need the status to control the ship
        public void ProvideScoutStatus(ScoutStatus ss)
        {
            gScoutStatus = ss;
        }

        // Method      : Sensors
        // Input       : Tachyon, Mass, RF,Visual,Radar (type:List<SenorInfo> - this is an array of variable length)
        // Output      : na
        // Description :
        //      This method is call 60 times a second. It provides an array of detected object 
        //      for each of the sensors.
        //      The number of items in the array is found by using the Count member,
        //      for example:
        //              numberOfItems = Tachyon.Count;
        //      You can then use a for loop to cycle through them all,
        //      for example:
        //              for (int i = 0; i < Tachyon.Count; i++)
        //              {
        //                  range = Tachyon[i].range;
        //                  angleToObject = Tachyon[i].angle;
        //                  objectType = Tachyon[i].objectType;
        //                  objectID = Tachyon[i].objectID;
        //              }
        public void Sensors(List<SensorInfo> Tachyon, List<SensorInfo> Mass, List<SensorInfo> RF, List<SensorInfo> Visual, List<SensorInfo> Radar)
        {
            Sensor lSensor = new Sensor(Tachyon, Mass, RF, Visual, Radar);
            gGPS.UpdateForeignObjectCoords(lSensor.mRelativeForeignObject);
        }

        // Method      : StartLevel
        // Input       : levelNumber (type: int)
        // Output      : na
        // Description :
        // This method is call once at the start of a game level and it give the games level.
        public void StartLevel(int levelNumber)
        {

        }

        // Method      : EndLevel
        // Input       : levelNumber (type: int), IsScoutAlive (type: bool)
        // Output      : na
        // Description :
        //      This method is call once at the end of a game level.
        //      It give the games level and if the scout survived the level.
        public void EndLevel(int levelNumber, bool IsScoutAlive)
        {

        }


        // Method      : InLevel
        // Input       : levelNumber (type: int)
        // Output      : na
        // Description :
        //      This method is call 60 times per second whilst in the game.
        //      It give the games level.
        public void InLevel(int levelNumber)
        {
        }

        // Method      : StartTask
        // Input       : task (type: int)
        // Output      : na
        // Description :
        // This method is call once at the start of a task and it give the task number.
        public void StartTask(int task)
        {
            InitializeControl();
            gGPS.InitializeScoutPose();
            
            gRotationalPID.InitializePID(-3, 3, 10000, 0, 0);
            gXPID.InitializePID(-1.5, 1.5, 10, 0, 0);
            gYPID.InitializePID(-3, 3, 10, 0, 0);

            mPath[0].X = 500;        // Generate a path of waypoints
            mPath[0].Y = 500;
            mPath[1].X = 500;
            mPath[1].Y = -500;
            mPath[2].X = -500;
            mPath[2].Y = -500;
            mPath[3].X = -500;
            mPath[3].Y = 500;
            mPath[4].X = 1000;
            mPath[4].Y = 1000;
            mPath[5].X = 1000;
            mPath[5].Y = -1000;
            mPath[6].X = -1000;
            mPath[6].Y = -1000;
            mPath[7].X = -1000;
            mPath[7].Y = 1000;
            mPathIndex = 0;

            mReportObject.complete = false;
            mReportObject.X = 0;
            mReportObject.Y = 0;

        }

        // Method      : EndTask
        // Input       : task (type: int), IsScoutAlive (type: bool)
        // Output      : na
        // Description :
        //      This method is call once at the end of a task.
        //      It give the task number and if the scout survived the task.
        public void EndTask(int task, bool IsScoutAlive)
        {

        }


        // Method      : InTask
        // Input       : task (type: int)
        // Output      : na
        // Description :
        //      This method is call 60 times per second whilst in the task.
        //      It give the task level.
        public void InTask(int task)
        {

            gGPS.TrackShip(gScoutStatus);
            mAvoidThrust.X = 0;
            mAvoidThrust.Y = 0;
            avoidObject();
            switch (task)
            {
                case 1:
                    InTask1();
                    break;
            }
        }

        #endregion

        #region FileCreation

        // Method      : CreateLogFile
        // Input       : na
        // Output      : na
        // Description :
        //      This method can be used to start a log file.
        //      This creates a text file and then writes the first few line of the file.
        //      If you put commas between the variable it will be a Comma Seperated File (CSV)
        //      and can be open in Excel later on.
        private void CreateLogFile()
        {
            sw = File.CreateText("MyLog.csv"); // Open log file
            sw.WriteLine("My Name");    // Write first line of log (usually name of log)
            sw.WriteLine("");           // Write a blank line
            sw.WriteLine("Velocity Forward,Velocity Sideways,Velocity Rotational");   // Write second line (usually column headings)

        }

        // Method      : LogData
        // Input       : na
        // Output      : na
        // Description :
        //      This method can be used to file data to a log file created in CreateLogFile.
        //      By putting commas between the variables you will get a CSV file.
        //      All types of data have the .ToString() member to convert the type to a string
        public void LogData()
        {
            //    sw.WriteLine(velForward.ToString() + "," +velRight.ToString() + "," + velRotation.ToString());  // Place variable that match the heading in CreateLogFile
        }

        // Method      : CloseLogFile
        // Input       : na
        // Output      : na
        // Description :
        //      This method will close the file opened in CreateLogFile.
        //      Use it when you have finished logging data.
        public void CloseLogFile()
        {
            try
            {
                sw.Close();
                sw.Dispose();
            }
            catch
            {
            }
        }

        #endregion

        private class PID
        {
       
                      
            // constants
            public double mPropConstant, mIntegralConstant, mDerivativeConstant;

            /// <summary>
            /// Overall constant, almost always set to 1.
            /// </summary>
            public double mOverallConstant;     
                  
            //Error values
            public double mPropError, mIntegralError, mDerivativeError;

            // saving the previous D error
            public double mLastDerivativeError;

            // Limits of the perscribed thrust
            public double mMaxOut, mMinOut;           

            /// <summary>
            /// Initializes the PID's attributes
            /// </summary>
            /// <param name="aProportionalConstant"></param>
            /// <param name="aIntegralConstant"></param>
            /// <param name="aDerivativeConstant"></param>
            public void InitializePID(double aMinOut, double aMaxOut, double aProportionalConstant, double aIntegralConstant, double aDerivativeConstant)
            {
                mDerivativeError = 0;
                mIntegralError = 0;
                mPropError = 0;
                mPropConstant = aProportionalConstant;
                mDerivativeConstant = aDerivativeConstant;
                mIntegralConstant = aIntegralConstant;
                mOverallConstant = 1;
                mLastDerivativeError = 0;
                mMinOut = aMinOut;
                mMaxOut = aMaxOut;
                
            }

            /// <summary>
            /// Calculates the required thrust to get to the target velocity.
            /// </summary>
            /// <param name="aTargetVelocity">The Target Velocity, the goal to get to.</param>
            /// <param name="aCurrentVelocity">The Current Velocity, how close the ship is to the goal already.</param>
            /// <returns>The thrust needed in order to reach the target velocity.</returns>
            public double CalculateThrust(double aTargetVelocity, double aCurrentVelocity)
            {
                double lThrustPerscription = 0;

                mPropError = aTargetVelocity - aCurrentVelocity; 
                mDerivativeError = mPropError - mLastDerivativeError;   
                mLastDerivativeError = mPropError;    


                lThrustPerscription = (mPropConstant * mPropError + mIntegralConstant * mIntegralError + mDerivativeConstant * mDerivativeError) / mOverallConstant;

                //Cap the thrust perscription
                if (lThrustPerscription > mMaxOut)      
                    lThrustPerscription = mMaxOut;
                else if (lThrustPerscription < mMinOut)
                    lThrustPerscription = mMinOut;

                //Get new Integral error and cap it as needed
                mIntegralError += mPropError;               
                if (mIntegralError > mMaxOut)        
                    mIntegralError = mMaxOut;
                if (mIntegralError < mMinOut)
                    mIntegralError = mMinOut;

                return lThrustPerscription;
            }


            
        }


        #region PriavteMethods

        /// <summary>
        /// An object in space that is represented in a global mapping.
        /// </summary>
        private class GlobalForeignObject
        {
            public int mObjectID;
            public ObjectType mTypeOfObject;

            public double mXCoord = 0;
            public double mYCoord = 0;

            /// <summary>
            /// Converts the Relative Foreign Object to a Global Foreign Object using the Scouts current position.
            /// If the range has not been detected, then a placeholder is inserted in order to make use of the GFO.
            /// </summary>
            /// <param name="aRFO"></param>
            /// <param name="aScoutPose"></param>
            /// <returns>The converted GFO</returns>
            public static GlobalForeignObject Convert (RelativeForeignObject aRFO, pose aScoutPose)
            {
                GlobalForeignObject lGFO = new GlobalForeignObject();

                lGFO.mObjectID = aRFO.mObjectID;
                lGFO.mTypeOfObject = aRFO.mTypeOfObject;
                if (!aRFO.FoundRange)
                {
                    switch (aRFO.mTypeOfObject)
                    {
                        case ObjectType.Distortion:  
                        case ObjectType.Asteroid:
                            aRFO.Range = 650;
                            break;
                        case ObjectType.BlackHole:
                            aRFO.Range = 550;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
                    

                lGFO.mXCoord = aRFO.Range * Math.Sin(aScoutPose.angle + aRFO.Angle) + aScoutPose.X;
                lGFO.mYCoord = aRFO.Range * Math.Cos(aScoutPose.angle + aRFO.Angle) + aScoutPose.Y;

                return lGFO;
            }

        }

        /// <summary>
        /// An object in space that is represented relative to the ship.
        /// </summary>
        private class RelativeForeignObject
        {
            public int mObjectID;
            public ObjectType mTypeOfObject;

            private int mInfoCount = 0;

            private double mAngle;
            public double Angle
            {
                set { mAngle = value; }
                get { return mAngle; }
            }

            private double mRange;
            public double Range
            {
                set { mInfoCount++; mRange = value; }
                get { return mRange; }
            }

            private bool mfoundRange = false;

            public bool FoundRange
            {
                get { return mfoundRange; }
            }



        }

        /// <summary>
        /// The class that handles all mapping.
        /// </summary>
        private class GPS
        {
            public pose mScoutPose = new pose();
            Dictionary<int, GlobalForeignObject> mGlobalForeignObjects = new Dictionary<int, GlobalForeignObject>();

            public void InitializeScoutPose()
            {
                mScoutPose.X = 0;
                mScoutPose.Y = 0;
                mScoutPose.angle = 0;
            }


            public void UpdateForeignObjectCoords(Dictionary<int,RelativeForeignObject> aRelativeForeignObjects)
            {
                foreach(KeyValuePair<int,RelativeForeignObject> iKeyValue in aRelativeForeignObjects)
                {
                    mGlobalForeignObjects[iKeyValue.Key] = GlobalForeignObject.Convert(iKeyValue.Value, mScoutPose);
                }
            }

            /// <summary>
            /// This method was created by the professor. 
            /// </summary>
            /// <param name="aScoutStatus"></param>
            public void TrackShip(ScoutStatus aScoutStatus)             // Use the current velocities to track the ship
            {
                double dx, dy, ndx, ndy;


                dy = aScoutStatus.averageVelocityForward * aScoutStatus.deltaTime;  // Distance moved forward since last time (use average velocity)
                dx = aScoutStatus.averageVelocityRight * aScoutStatus.deltaTime;    // Distance moved sideways since last time

                mScoutPose.angle += aScoutStatus.currentVelocityAngularCW * (aScoutStatus.deltaTime / 2);  // Get half of the angle rotated through since last time
                                                                                                           // (This is the average angle and is used to make the tracking more accurate)
                mScoutPose.angle = mScoutPose.angle % Math.PI * 2;          // ensure to angle is between -2*PI ans 2*PI

                ndx = dx * Math.Cos(mScoutPose.angle) + dy * Math.Sin(mScoutPose.angle);  // Rotate the forward and sideways distances to world coords
                ndy = -dx * Math.Sin(mScoutPose.angle) + dy * Math.Cos(mScoutPose.angle);
                mScoutPose.X += ndx;                             // Update the scouts coordinates
                mScoutPose.Y += ndy;

                mScoutPose.angle += aScoutStatus.currentVelocityAngularCW * (aScoutStatus.deltaTime / 2);    // Get the last half of the angle rotated through
                mScoutPose.angle = mScoutPose.angle % Math.PI * 2;          // ensure to angle is between -2*PI ans 2*PI

            }
        }


        private class Sensor
        {

            private Dictionary<int, RelativeForeignObject> RelativeForeignObject = new Dictionary<int, RelativeForeignObject>();
            
            public Dictionary<int, RelativeForeignObject> mRelativeForeignObject
            {
                get
                {
                    return RelativeForeignObject;
                }
            }



            public Sensor(List<SensorInfo> Tachyon, List<SensorInfo> Mass, List<SensorInfo> RF, List<SensorInfo> Visual, List<SensorInfo> Radar)
            {
                /*
                foreach(SensorInfo iTachyon in Tachyon)
                {

                    mRelativeForeignObject[iTachyon.objectID].mRange = iTachyon.range;
                    mRelativeForeignObject[iTachyon.objectID].mTypeOfObject = iTachyon.objectType;
                }
                */

                //only these are needed for black hole location.

                foreach(SensorInfo iMass in Mass)
                {
                    //could cause problems. This code might not work for a key that doesn't exist
                    mRelativeForeignObject[iMass.objectID].Range = iMass.range;
                    mRelativeForeignObject[iMass.objectID].mTypeOfObject = iMass.objectType;

                }

                foreach(SensorInfo iRF in RF)
                {
                    mRelativeForeignObject[iRF.objectID].Angle = iRF.angle;
                    mRelativeForeignObject[iRF.objectID].mTypeOfObject = iRF.objectType;
                }
            }
        }




        


        private void MoveToTarget(double targetX, double targetY, double maxVelocity)
        {
            double deltaX, deltaY;

            deltaX = targetX - gGPS.mScoutPose.X;
            deltaY = targetY - gGPS.mScoutPose.Y;

            double angleToFace = Math.Atan2(deltaX, deltaY);

            angleToFace = angleToFace % TwoPI;

            double anglebetweenFaceAndCurrentHeading = angleToFace - gGPS.mScoutPose.angle;

            anglebetweenFaceAndCurrentHeading = anglebetweenFaceAndCurrentHeading % TwoPI;

            if (anglebetweenFaceAndCurrentHeading < -Math.PI)
                anglebetweenFaceAndCurrentHeading += TwoPI;
            if (anglebetweenFaceAndCurrentHeading > Math.PI)
                anglebetweenFaceAndCurrentHeading -= TwoPI;

            double requiredCWVel = anglebetweenFaceAndCurrentHeading / 64;

            if (requiredCWVel > 0.005f)
                requiredCWVel = 0.005f;
            if (requiredCWVel < -0.005)
                requiredCWVel = -0.005;

            gScoutControl.ThrustCW = gRotationalPID.CalculateThrust(requiredCWVel, gScoutStatus.currentVelocityAngularCW);



            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY); //Distance to waypoint
            double newdX, newdY;
            double requiredXVel, requiredYVel;


            newdX = distance * Math.Sin(anglebetweenFaceAndCurrentHeading); // Get the sideways distance the scout must move
            newdY = distance * Math.Cos(anglebetweenFaceAndCurrentHeading); // Get the forward distance the scout must move

            requiredXVel = newdX / 200;     // The required velocity is slower as the scout gets closer to the destination
            requiredYVel = newdY / 200;

            gScoutControl.ThrustRight = gXPID.CalculateThrust(requiredXVel, gScoutStatus.currentVelocityRight);

            if (requiredYVel > maxVelocity)
                requiredYVel = maxVelocity;

            gScoutControl.ThrustForward = gYPID.CalculateThrust(requiredYVel, gScoutStatus.currentVelocityForward);
        }

        private void MoveToWaypoint(double MaxVel)
        {
            pose targetPose = new pose();

            targetPose.X = mPath[mPathIndex].X;
            targetPose.Y = mPath[mPathIndex].Y;

            double deltaX, deltaY;

            deltaX = targetPose.X - gGPS.mScoutPose.X;
            deltaY = targetPose.Y - gGPS.mScoutPose.Y;

            double dist;

            dist = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));

            if (dist < 50)
            {
                mPathIndex++;
                if (mPathIndex > 7)
                    mPathIndex = 0;


                targetPose.X = mPath[mPathIndex].X;
                targetPose.Y = mPath[mPathIndex].Y;
            }
            MoveToTarget(targetPose.X, targetPose.Y, MaxVel);
        }


        //
        private void InTask1()
        {
            MoveToWaypoint(0.4);


            gScoutControl.MinerOn = false;
            gScoutControl.ShieldOn = false;
            gScoutControl.EnergyExtractorOn = false;
        }

        private pose RotateAboutZ(double x, double y, double angle) // Rotate a vector clockwise through a given angle
        {
            pose p;
            p.angle = angle;
            p.X = x * Math.Cos(angle) + y * Math.Sin(angle);
            p.Y = -x * Math.Sin(angle) + y * Math.Cos(angle);

            /*
             * The following is the rotation for rotating through an anti-clockwise direction and is found in most text books
             * X' = xCosA - ySinA
             * Y' = xSinA + yCosA
             * 
             * But we are rotating in a clockwise direction so the rotational equations become;
             * X' = xCosA + ySinA
             * Y' = -xCosA + ySinA
            */
            return p;
        }

        private void avoidObject()
        {
            double range = 0;
            double thrust = 0;
            pose athrust = new pose();

            for (int i = 0; i < 11; i++)
            {
                if ((mBlackHoles[i].X != 0) && (mBlackHoles[i].Y != 0)) // check for blank or no blackhole
                {
                    range = Math.Sqrt(Math.Pow(gScoutPose.X - mBlackHoles[i].X, 2) + Math.Pow(gScoutPose.Y - mBlackHoles[i].Y, 2));
                    if (range < 200)
                    {
                        thrust = 600 / range;
                        athrust = RotateAboutZ(0, thrust, mBlackHoles[i].angle - Math.PI);
                        mAvoidThrust.X += athrust.X;
                        mAvoidThrust.Y += athrust.Y;
                    }
                }
            }
        }

        public void InitializeControl()
        {
            gScoutControl.ThrustCW = 0;
            gScoutControl.ThrustForward = 0;
            gScoutControl.ThrustRight = 0;
            gScoutControl.ShieldOn = false;
            gScoutControl.MinerOn = false;
            gScoutControl.EnergyExtractorOn = false;
        }

        #endregion


    }
}