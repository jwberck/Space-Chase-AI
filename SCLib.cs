using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SpaceChaseLib

{
    #region Structs
    public struct StudentInfo           // This structure is used to hold the students information
    {
        public string studentLastName;      // Student's last name
        public string studentFirstName;     // Student's first name
        public string studentIdNumber;      // Student's ID number
        public string studentCourse;        // Students's course code
    }

    public struct GameStatus            // This structure holds the game status
    {
        public int GameLevel;           // current game level (zero if a task is being performed or in menu)
        public int TaskLevel;           // current task (zero if a game is playing or in menu)
        public int iteration;           // the number of times this task/game has been repeated
        public double LevelTimeMS;      // time taken so far in the game level or task
        public double TotalTimeMS;      // total game time taken for all levels
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

    // ObjectType is an enumeration of the different object types the ship's sensors can detect
    public enum ObjectType { Asteroid, BlackHole, Distortion, CombatDrone, Factory }

    public struct SensorInfo                    // contains the information gathered from a sensor
    {
        public double range;                    // The range to the detected object
        public double angle;                    // The angle from the front of the scout ship to the object
        public ObjectType objectType;           // This is the type of object detected
        public int objectID;                    // This is a unique ID for this object in its type
    }

    public struct report                        // This structre is used in task one to report on the blackhole location
    {
        public bool complete;                   // When this is set to true the task is complete
        public double X;                        // This has to hold the X coord of the BlackHole
        public double Y;                        // This has to hold the Y coord of the BlackHole
    }

    public struct PID_Params                // This structure is used to run a PID controller
    {
        public double input;                    // The current value
        public double kP, kI, kD, kO;           // constants
        public double eP, eI, eD;               // errors
        public double setPoint;                 // The value we wish to obtain
        public double lastErrorD;               // saving the previous D error
        public double output;                   // output is feedback to the object being controlled
        public double maxOut, minOut;           // Limits of the output
    }

    public struct pose
    {
        public double X;
        public double Y;
        public double angle;
    }
    #endregion

    public class SCLib
    {
        // "m" represents member variables
        // TODO: Add your global variables here
        ScoutControl mScoutControl = new ScoutControl();
        pose mScoutPose = new pose();
        ScoutStatus mScoutStatus = new ScoutStatus();
        double TwoPI = Math.PI * 2;
        PID_Params mPIDParamsR = new PID_Params();
        PID_Params mPIDParamsX = new PID_Params();   // holds the PID parameters for the Sideways motion PID
        PID_Params mPIDParamsY = new PID_Params();   // holds the PID parameters for the forwards/backwards motion PID

        pose[] mPath = new pose[20];                 // Used to hold a series of waypoints for the scount to follow
        int mPathIndex;                              // Used to hold the current waypoint

        report mReportObject = new report();

        pose mBlackHolePose = new pose();

        pose[] mBlackHoles = new pose[11];
        SensorInfo[] mBlackholesSensor = new SensorInfo[11];

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

            strMessage = "Ship X :" + mScoutPose.X.ToString() + "\nShip Y : " + mScoutPose.Y.ToString() + "\nShip A : " + mScoutPose.angle.ToString() + "\nBH X = " + mBlackHolePose.X.ToString() + "\nBH Y = " + mBlackHolePose.Y.ToString();

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
            //ScoutControl t = new ScoutControl();

            //t.ThrustForward = 1;            // Replace the value with the forward thrust
            //t.ThrustRight = 1;              // Replace the value with the sideways thrust
            //t.ThrustCW = 1;                 // Replace the value with the rotational thrust
            //t.ShieldOn = false;             // Set to true to turn the shields on
            //t.EnergyExtractorOn = false;    // Set to true to turn on the Energy Extractor
            //t.MinerOn = false;              // Set to true to turn on the Miner.

            return mScoutControl;
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
            //report r = new report();    // set up a report structure to return

            //r.complete = false;         // Set to true when the report is ready
            //r.X = 0;                    // Set to the blackhole's X coord
            //r.Y = 0;                    // Set to the blackhole's Y coord

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
        }


        // Method     : ProvideScoutStatus
        // Input      : ss (type: ScoutStatus)
        // Output     : na
        // Dscription :
        //      This method is call 60 times per second and provides the status of the scout ship.
        //      You will need the status to control the ship
        public void ProvideScoutStatus(ScoutStatus ss)
        {
            mScoutStatus = ss;
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
            int numberOfItems;

            numberOfItems = RF.Count;

            double BHrange = 0;
            double BHangle = 0;

            for (int i = 0; i < 11; i++)
            {
                mBlackholesSensor[i].angle = 0;
                mBlackholesSensor[i].range = 0;
            }
            for (int i = 0; i < numberOfItems; i++)
            {
                if (RF[i].objectType == ObjectType.BlackHole)
                {
                    BHangle = RF[i].angle;
                    mBlackholesSensor[RF[i].objectID].angle = RF[i].angle;
                }
            }

            foreach (SensorInfo m in Mass)
            {
                if (m.objectType == ObjectType.BlackHole)
                {
                    BHrange = m.range;
                    mBlackholesSensor[m.objectID].range = m.range;
                }
            }

            for (int i = 0; i < 11; i++)
            {
                mBlackHolePose = GetCoords(mScoutPose.X, mScoutPose.Y, mScoutPose.angle, mBlackholesSensor[i].angle, mBlackholesSensor[i].range);
                if ((mBlackHolePose.X != mScoutPose.X) && (mBlackHolePose.Y != mScoutPose.Y))
                {
                    mBlackHoles[i] = mBlackHolePose;
                    mBlackHoles[i].angle = mBlackholesSensor[i].angle; //store local angle. Only accurate during this current cycle
                }
            }

            if (BHrange != 0)
            {
                mBlackHolePose = GetCoords(mScoutPose.X, mScoutPose.Y, mScoutPose.angle, BHangle, BHrange);
                mReportObject.X = mBlackHolePose.X;
                mReportObject.Y = mBlackHolePose.Y;
                //  reportObject.complete = true;
            }
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
            mScoutControl.ThrustCW = 0;
            mScoutControl.ThrustForward = 0;
            mScoutControl.ThrustRight = 0;
            mScoutControl.ShieldOn = false;
            mScoutControl.MinerOn = false;
            mScoutControl.EnergyExtractorOn = false;

            mScoutPose.X = 0;
            mScoutPose.Y = 0;
            mScoutPose.angle = 0;

            mPIDParamsR = PIDParametersSet(10000, 0, 0, 1, -3, 3, 0, 0);
            mPIDParamsX = PIDParametersSet(10, 0, 0, 1, -3, 3, 0, 0);
            mPIDParamsY = PIDParametersSet(10, 0, 0, 1, -3, 3, 0, 0);

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

            for (int i = 0; i < 11; i++)
            {
                mBlackHoles[i].X = 0;
                mBlackHoles[i].Y = 0;
            }
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
            TrackShip();
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

        #region PIDMethods

        // Method      : PIDController
        // Input       : pidP (type: PID_Params)
        // Output      : (type: PID_Params)
        // Description :
        //      This method will run one step of a PID controller.
        //      It accepts PID parameters from the previous step and a control input (set point).
        //      It returns the PID parameters for the next step and the control output.

        private PID_Params PIDController(PID_Params pidP)
        {
            pidP.eP = pidP.setPoint - pidP.input;  // P error

            pidP.eD = pidP.eP - pidP.lastErrorD;   // D error
            pidP.lastErrorD = pidP.eP;            // Save last error;

            pidP.output = (pidP.kP * pidP.eP + pidP.kI * pidP.eI + pidP.kD * pidP.eD) / pidP.kO;

            if (pidP.output > pidP.maxOut)      // cap the control output
                pidP.output = pidP.maxOut;
            else if (pidP.output < pidP.minOut)
                pidP.output = pidP.minOut;

            pidP.eI += pidP.eP;               // I error
            if (pidP.eI > pidP.maxOut)        // cap the I error
                pidP.eI = pidP.maxOut;
            if (pidP.eI < pidP.minOut)
                pidP.eI = pidP.minOut;

            return pidP;
        }

        //Method        : PIDParametersSet
        //Input         : pidP, (type PI_Params) P,I,D,O,Min,Max,Setpoint,input (all type double)
        //Output        : (type: PID_Params)
        //Description   : 
        //      Set the PID Constants in pidP
        private PID_Params PIDParametersSet(PID_Params pidP, double P, double I, double D, double O, double Min, double Max, double Setpoint, double input)
        {
            PID_Params p;
            p = pidP;

            p.input = input;
            p.kD = D;
            p.kI = I;
            p.kO = O;
            p.kP = P;
            p.maxOut = Max;
            p.minOut = Min;
            p.setPoint = Setpoint;

            return p;
        }


        //Method        : PIDParametersSet
        //Input         : P,I,D,O,Min,Max,Setpoint,input (all type double)
        //Output        : (type: PID_Params)
        //Description   : 
        //      Set the PID Constants
        private PID_Params PIDParametersSet(double P, double I, double D, double O, double Min, double Max, double Setpoint, double input)
        {
            PID_Params p;

            p.eD = 0;
            p.eI = 0;
            p.eP = 0;
            p.input = 0;
            p.kD = D;
            p.kI = I;
            p.kO = O;
            p.kP = P;
            p.lastErrorD = 0;
            p.maxOut = Max;
            p.minOut = Min;
            p.output = 0;
            p.setPoint = Setpoint;

            return p;
        }

        #endregion

        #region PriavteMethods

        private void TrackShip()             // Use the current velocities to track the ship
        {
            double dx, dy, ndx, ndy;


            dy = mScoutStatus.averageVelocityForward * mScoutStatus.deltaTime;  // Distance moved forward since last time (use average velocity)
            dx = mScoutStatus.averageVelocityRight * mScoutStatus.deltaTime;    // Distance moved sideways since last time

            mScoutPose.angle += mScoutStatus.currentVelocityAngularCW * (mScoutStatus.deltaTime / 2);  // Get half of the angle rotated through since last time
                                                                                      // (This is the average angle and is used to make the tracking more accurate)
            mScoutPose.angle = mScoutPose.angle % TwoPI;          // ensure to angle is between -2*PI ans 2*PI

            ndx = dx * Math.Cos(mScoutPose.angle) + dy * Math.Sin(mScoutPose.angle);  // Rotate the forward and sideways distances to world coords
            ndy = -dx * Math.Sin(mScoutPose.angle) + dy * Math.Cos(mScoutPose.angle);
            mScoutPose.X += ndx;                             // Update the scouts coordinates
            mScoutPose.Y += ndy;

            mScoutPose.angle += mScoutStatus.currentVelocityAngularCW * (mScoutStatus.deltaTime / 2);    // Get the last half of the angle rotated through
            mScoutPose.angle = mScoutPose.angle % TwoPI;          // ensure to angle is between -2*PI ans 2*PI

        }

        private void MoveToTarget(double targetX, double targetY, double maxVelocity)
        {
            double deltaX, deltaY;

            deltaX = targetX - mScoutPose.X;
            deltaY = targetY - mScoutPose.Y;

            double angleToFace = Math.Atan2(deltaX, deltaY);

            angleToFace = angleToFace % TwoPI;

            double anglebetweenFaceAndCurrentHeading = angleToFace - mScoutPose.angle;

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
            mPIDParamsR = PIDParametersSet(mPIDParamsR, 10000, 0, 0, 1, -3, 3, requiredCWVel, mScoutStatus.currentVelocityAngularCW);
            mPIDParamsR = PIDController(mPIDParamsR);

            mScoutControl.ThrustCW = mPIDParamsR.output;


            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY); //Distance to waypoint
            double newdX, newdY;
            double requiredXVel, requiredYVel;


            newdX = distance * Math.Sin(anglebetweenFaceAndCurrentHeading); // Get the sideways distance the scout must move
            newdY = distance * Math.Cos(anglebetweenFaceAndCurrentHeading); // Get the forward distance the scout must move

            requiredXVel = newdX / 200;     // The required velocity is slower as the scout gets closer to the destination
            requiredYVel = newdY / 200;

            mPIDParamsX = PIDParametersSet(mPIDParamsX, 10, 0, 0, 1, -1.5, 1.5, requiredXVel, mScoutStatus.currentVelocityRight);
            mPIDParamsX = PIDController(mPIDParamsX);   // Call the PID


            mScoutControl.ThrustRight = mPIDParamsX.output + mAvoidThrust.X;

            if (requiredYVel > maxVelocity)
                requiredYVel = maxVelocity;

            mPIDParamsY = PIDParametersSet(mPIDParamsY, 10, 0, 0, 1, -3, 3, requiredYVel, mScoutStatus.currentVelocityForward);

            mPIDParamsY = PIDController(mPIDParamsY);   // Call the PID

            mScoutControl.ThrustForward = mPIDParamsY.output + mAvoidThrust.Y;

        }

        private void MoveToWaypoint(double MaxVel)
        {
            pose targetPose = new pose();

            targetPose.X = mPath[mPathIndex].X;
            targetPose.Y = mPath[mPathIndex].Y;

            double deltaX, deltaY;

            deltaX = targetPose.X - mScoutPose.X;
            deltaY = targetPose.Y - mScoutPose.Y;

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

        private pose GetCoords(double sX, double sY, double sA, double angle, double range)
        {
            pose coords;

            coords.angle = 0;

            coords.X = range * Math.Sin(sA + angle) + sX;
            coords.Y = range * Math.Cos(sA + angle) + sY;

            return (coords);
        }
        //
        private void InTask1()
        {
            MoveToWaypoint(0.4);


            mScoutControl.MinerOn = false;
            mScoutControl.ShieldOn = false;
            mScoutControl.EnergyExtractorOn = false;
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
                    range = Math.Sqrt(Math.Pow(mScoutPose.X - mBlackHoles[i].X, 2) + Math.Pow(mScoutPose.Y - mBlackHoles[i].Y, 2));
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

        #endregion
    }
}