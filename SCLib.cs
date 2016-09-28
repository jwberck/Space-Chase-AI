using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SpaceChaseLib

{

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


    public class pose
    {
        public double X = 0;
        public double Y = 0;
        public double angle = 0;
    }


    public class SCLib
    {

        #region Never touch again

        private Brain gBrain = new Brain();








        // Method      : StudentDetails
        // Input       : na
        // Output      : SD (type:StudentInfo)
        // Description :
        //      This method is called once when the program is first run 
        //      and is used to record the students details
        public StudentInfo StudentDetails()
        {
            StudentInfo SD = new StudentInfo();

            SD.studentLastName = "Berck";    // Replace the string with your last name
            SD.studentFirstName = "James";  // Replace the string with your first name
            SD.studentIdNumber = "216314405";    // Replace the string with your student number
            SD.studentCourse = "X009S";    // Replace the string with your course code

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
            ScoutControl lScoutControl = new ScoutControl();

            lScoutControl.ThrustForward = gBrain.mScoutThrustControls.ThrustForward;
            lScoutControl.ThrustRight = gBrain.mScoutThrustControls.ThrustRight;
            lScoutControl.ThrustCW = gBrain.mScoutThrustControls.ThrustCW;

            lScoutControl.MinerOn = gBrain.mScoutActionControls.MinerOn;
            lScoutControl.ShieldOn = gBrain.mScoutActionControls.ShieldOn;
            lScoutControl.EnergyExtractorOn = gBrain.mScoutActionControls.EnergyExtractorOn;

            return lScoutControl;



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
            gBrain.mGameStatus = gs;
        }


        // Method     : ProvideScoutStatus
        // Input      : ss (type: ScoutStatus)
        // Output     : na
        // Dscription :
        //      This method is call 60 times per second and provides the status of the scout ship.
        //      You will need the status to control the ship
        public void ProvideScoutStatus(ScoutStatus ss)
        {


            //Convert the struct to a class so that it can be used as a reference type
            gBrain.mScoutState.deltaTime = ss.deltaTime;
            gBrain.mScoutState.deltaVelocityForward = ss.deltaVelocityForward;
            gBrain.mScoutState.deltaVelocityRight = ss.deltaVelocityRight;
            gBrain.mScoutState.deltaVelocityAngularCW = ss.deltaVelocityAngularCW;
            gBrain.mScoutState.currentVelocityForward = ss.currentVelocityForward;
            gBrain.mScoutState.currentVelocityRight = ss.currentVelocityRight;
            gBrain.mScoutState.currentVelocityAngularCW = ss.currentVelocityAngularCW;
            gBrain.mScoutState.averageVelocityForward = ss.averageVelocityForward;
            gBrain.mScoutState.averageVelocityRight = ss.averageVelocityRight;
            gBrain.mScoutState.averageVelocityAngularCW = ss.averageVelocityAngularCW;
            gBrain.mScoutState.shieldEnergy = ss.shieldEnergy;
            gBrain.mScoutState.hullIntegrity = ss.hullIntegrity;


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
            gBrain.mMap.UpdateMap(lSensor.RelativeForeignObjects);
        }

        public String ScreenMessage()
        {
            return gBrain.ScreenMessage();
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

            return gBrain.Report();
        }


        // Method      : InitialiseGame
        // Input       : na
        // Output      : na
        // Description :
        //      This method is called once when the program is first run 
        //      and can be use to initialise any variables if required
        public void InitialiseGame()
        {
            gBrain.InitialiseGame();
        }

        // Method      : StartLevel
        // Input       : levelNumber (type: int)
        // Output      : na
        // Description :
        // This method is call once at the start of a game level and it give the games level.
        public void StartLevel(int levelNumber)
        {
            gBrain.StartLevel(levelNumber);
        }

        // Method      : EndLevel
        // Input       : levelNumber (type: int), IsScoutAlive (type: bool)
        // Output      : na
        // Description :
        //      This method is call once at the end of a game level.
        //      It give the games level and if the scout survived the level.
        public void EndLevel(int levelNumber, bool IsScoutAlive)
        {
            gBrain.EndLevel(levelNumber, IsScoutAlive);
        }


        // Method      : InLevel
        // Input       : levelNumber (type: int)
        // Output      : na
        // Description :
        //      This method is call 60 times per second whilst in the game.
        //      It give the games level.
        public void InLevel(int levelNumber)
        {
            gBrain.InLevel(levelNumber);
        }

        // Method      : StartTask
        // Input       : task (type: int)
        // Output      : na
        // Description :
        // This method is call once at the start of a task and it give the task number.
        public void StartTask(int task)
        {
            gBrain.StartTask(task);
        }

        // Method      : EndTask
        // Input       : task (type: int), IsScoutAlive (type: bool)
        // Output      : na
        // Description :
        //      This method is call once at the end of a task.
        //      It give the task number and if the scout survived the task.
        public void EndTask(int task, bool IsScoutAlive)
        {
            gBrain.EndTask(task, IsScoutAlive);
        }


        // Method      : InTask
        // Input       : task (type: int)
        // Output      : na
        // Description :
        //      This method is call 60 times per second whilst in the task.
        //      It give the task level.
        public void InTask(int task)
        {

            gBrain.InTask(task);
        }

        #endregion


        public class Sensor
        {
            enum ModifiedValue { Angle, Range, Both }

            private List<RelativeForeignObject> mRelativeForeignObjects = new List<RelativeForeignObject>();

            public List<RelativeForeignObject> RelativeForeignObjects
            {
                get
                {
                    return mRelativeForeignObjects;
                }
            }

            private void AddRelativeForeignObject(SensorInfo aSensorInfo, ModifiedValue aModifiedValue)
            {
                bool lAlreadyExists = false;
                int lIndexOfRFO = 0;

                //Checks to see if the object exists already and assings its index to lIndexOfRFO.
                foreach (RelativeForeignObject iRelativeForeignObject in mRelativeForeignObjects)
                {

                    if (iRelativeForeignObject.mObjectID == aSensorInfo.objectID && iRelativeForeignObject.mTypeOfObject == aSensorInfo.objectType)
                    {
                        lAlreadyExists = true;
                        break;
                    }
                    lIndexOfRFO++;
                }

                //Adds a RelativeForeignObject from the given SensorInfo
                if (lAlreadyExists)
                {
                    if (aModifiedValue == ModifiedValue.Range)
                        mRelativeForeignObjects[lIndexOfRFO].Range = aSensorInfo.range;
                    else if (aModifiedValue == ModifiedValue.Angle)
                        mRelativeForeignObjects[lIndexOfRFO].Angle = aSensorInfo.angle;
                    else
                    {
                        mRelativeForeignObjects[lIndexOfRFO].Range = aSensorInfo.range;
                        mRelativeForeignObjects[lIndexOfRFO].Angle = aSensorInfo.angle;
                    }
                }
                else
                {
                    RelativeForeignObject lRFOToBeAdded = new RelativeForeignObject();
                    lRFOToBeAdded.mTypeOfObject = aSensorInfo.objectType;
                    lRFOToBeAdded.mObjectID = aSensorInfo.objectID;

                    if (aModifiedValue == ModifiedValue.Range)
                        lRFOToBeAdded.Range = aSensorInfo.range;
                    else if (aModifiedValue == ModifiedValue.Angle)
                        lRFOToBeAdded.Angle = aSensorInfo.angle;
                    else
                    {
                        lRFOToBeAdded.Range = aSensorInfo.range;
                        lRFOToBeAdded.Angle = aSensorInfo.angle;
                    }

                    mRelativeForeignObjects.Add(lRFOToBeAdded);
                }
            }


            public Sensor(List<SensorInfo> Tachyon, List<SensorInfo> Mass, List<SensorInfo> RF, List<SensorInfo> Visual, List<SensorInfo> Radar)
            {


                foreach (SensorInfo iTachyon in Tachyon)
                {
                    AddRelativeForeignObject(iTachyon, ModifiedValue.Range);
                }

                foreach (SensorInfo iMass in Mass)
                {
                    AddRelativeForeignObject(iMass, ModifiedValue.Range);
                }

                foreach (SensorInfo iRF in RF)
                {
                    AddRelativeForeignObject(iRF, ModifiedValue.Angle);
                }

                foreach (SensorInfo iVisual in Visual)
                {
                    AddRelativeForeignObject(iVisual, ModifiedValue.Angle);
                }

                foreach (SensorInfo iRadar in Radar)
                {
                    AddRelativeForeignObject(iRadar, ModifiedValue.Both);

                }
            }
        }

        /// <summary>
        /// An object in space that is represented relative to the ship.
        /// </summary>
        public class RelativeForeignObject
        {
            public int mObjectID;
            public ObjectType mTypeOfObject;

            private bool mFoundRange = false;


            private double mAngle;
            public double Angle
            {
                set { mAngle = value; }
                get { return mAngle; }
            }

            private double mRange;
            public double Range
            {
                set { mFoundRange = true; mRange = value; }
                get { return mRange; }
            }


            public bool FoundRange
            {
                get { return mFoundRange; }
            }
        }



        private StreamWriter sw; //This holds the file to log data to in CSV format

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

        public class ScoutThrustControls
        {
            public double ThrustForward = 0;    // This is the forward thrust (-ve thrust is rearward)
            public double ThrustRight = 0;      // This is sideways, or strafing thrust (+ve i to the right of the scout)
            public double ThrustCW = 0;         // This is angular/rotational thrust (+ve is clockwise) 
        }

        public class ScoutActionControls
        {
            public bool ShieldOn;           // This is set to true if you want the shields to be on
            public bool EnergyExtractorOn;  // This is set to true if you want the Energy Extractor to be operating
            public bool MinerOn;            // This is set to true if you want the Miner to be operating
        }


        public class ScoutState
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
        }


        private class Brain
        {
            //World Status, updated every frame
            public ScoutState mScoutState = new ScoutState();
            public GameStatus mGameStatus = new GameStatus();
            public Map mMap = new Map();

            //Scout Control, outputs every frame
            public ScoutThrustControls mScoutThrustControls = new ScoutThrustControls();
            public ScoutActionControls mScoutActionControls = new ScoutActionControls();


            public Navigation mNavigation = new Navigation();
            report mReportObject = new report();

            //hacks
            public bool isMinerFinished = false;
            private bool isJavelinActive = false;


            /// <summary>
            /// Sets up references, bad practice. Attempt to seperate if possible.
            /// </summary>
            public void InitialiseGame()
            {
                mNavigation.Initialize(mMap);
            }



            public void StartTask(int task)
            {
                Reset();
                switch (task)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        mNavigation.CreateBoxPath();
                        break;

                }

            }

            public void InTask(int task)
            {

                mMap.TrackShip(mScoutState);
                switch (task)
                {
                    case 1:
                        InTask1();
                        break;
                    case 2:
                        InTask2();
                        break;
                    case 3:
                        InTask3();
                        break;
                    case 4:
                        InTask4();
                        break;
                    case 5:
                        InTask5();
                        break;

                }
            }
            private void InTask1()
            {

                if (mMap.mBlackHoles.Count >= 1)
                {
                    pose lBlackHolePose = new pose();
                    lBlackHolePose.X = mMap.mBlackHoles.First().Value.mXCoord;
                    lBlackHolePose.Y = mMap.mBlackHoles.First().Value.mYCoord;


                    //mNavigation.ReplaceWaypointAtFront(lPose);
                    mNavigation.AddWaypointToFront(lBlackHolePose);
                    if (mMap.CalculateDistanceFromScout(lBlackHolePose.X, lBlackHolePose.Y) < 100)
                    {
                        mReportObject.X = lBlackHolePose.X;
                        mReportObject.Y = lBlackHolePose.Y;
                        mReportObject.complete = true;
                        return;
                    }
                }
                ScoutThrustControls lScoutThrust = mNavigation.MoveToWaypoint();
                mScoutThrustControls = mNavigation.ApplyPID(lScoutThrust, mScoutState);

            }

            private void InTask2()
            {
                if (mMap.mAsteroids.Count > 0)
                {
                    //This is super verbose, try to slim it down.
                    //Gets collection point and distance to that point
                    pose lCollectionPoint = mMap.CalculateOrbitPoint(mMap.mAsteroids.First().Value.mXCoord, mMap.mAsteroids.First().Value.mYCoord);
                    double lDistanceToCollectionPoint = mMap.CalculateDistanceFromScout(lCollectionPoint.X, lCollectionPoint.Y);

                    mNavigation.ReplaceWaypointAtFront(lCollectionPoint);

                    //Reports the asteroid when close
                    if (!mReportObject.complete && lDistanceToCollectionPoint < 100)
                    {
                        mReportObject.complete = true;
                    }

                    //Turns on the miner if close enough
                    if (lDistanceToCollectionPoint < 15)
                    {
                        mScoutActionControls.MinerOn = true;
                    }
                    think(500, 1);

                }
                else
                {
                    think();
                }
            }

            private void InTask3()
            {
                if (mMap.mAsteroids.Count > 0 && isMinerFinished == false)
                {
                    //This is super verbose, try to slim it down.
                    //Gets collection point and distance to that point
                    pose lCollectionPoint = mMap.CalculateOrbitPoint(mMap.mAsteroids.First().Value.mXCoord, mMap.mAsteroids.First().Value.mYCoord);
                    double lDistanceToCollectionPoint = mMap.CalculateDistanceFromScout(lCollectionPoint.X, lCollectionPoint.Y);

                    mNavigation.ReplaceWaypointAtFront(lCollectionPoint);

                    //Turns on the miner if close enough
                    if (lDistanceToCollectionPoint < 15)
                    {
                        mScoutActionControls.MinerOn = true;
                    }
                    if (mScoutState.hullIntegrity == 1)
                    {
                        mScoutActionControls.MinerOn = false;
                        isMinerFinished = true;
                    }
                    think(500, 1);
                }
                else if (mMap.mDistortions.Count > 0 && isMinerFinished == true)
                {
                    pose lCollectionPoint = mMap.CalculateOrbitPoint(mMap.mDistortions.First().Value.mXCoord, mMap.mDistortions.First().Value.mYCoord);
                    double lDistanceToCollectionPoint = mMap.CalculateDistanceFromScout(lCollectionPoint.X, lCollectionPoint.Y);

                    mNavigation.ReplaceWaypointAtFront(lCollectionPoint);

                    //Turns on the miner if close enough
                    if (lDistanceToCollectionPoint < 15)
                    {
                        mReportObject.complete = true;
                        mScoutActionControls.EnergyExtractorOn = true;
                    }

                    if (mScoutState.shieldEnergy == 1)
                    {
                        mScoutActionControls.ShieldOn = true;
                    }
                    think(500, 1);
                }
                else
                {
                    think();
                }


            }

            private void InTask4()
            {
                think();

            }

            private void InTask5()
            {
                think();
            }


            public void EndTask(int task, bool IsScoutAlive)
            {
                Reset();
            }



            public void StartLevel(int levelNumber)
            {
                Reset();
                mNavigation.CreateBoxPath();
            }

            public void InLevel(int levelNumber)
            {
                mMap.TrackShip(mScoutState);
                InTask4();
            }

            public void EndLevel(int levelNumber, bool IsScoutAlive)
            {
                Reset();
            }
            public String ScreenMessage()
            {
                StringBuilder SBMessage = new StringBuilder();

                SBMessage.Append("Ship X :" + mMap.mScoutPose.X.ToString() + "\nShip Y : " + mMap.mScoutPose.Y.ToString() + "\nShip A : " + mMap.mScoutPose.angle.ToString());

                foreach (KeyValuePair<int, GlobalForeignObject> iGFOKeyValue in mMap.mBlackHoles)
                {
                    SBMessage.Append("\n Blackhole " + iGFOKeyValue.Key.ToString() + ": ( " + iGFOKeyValue.Value.mXCoord.ToString() + " , " + iGFOKeyValue.Value.mYCoord.ToString() + " )");
                }

                foreach (KeyValuePair<int, GlobalForeignObject> iGFOKeyValue in mMap.mAsteroids)
                {
                    SBMessage.Append("\n Asteroid " + iGFOKeyValue.Key.ToString() + ": ( " + iGFOKeyValue.Value.mXCoord.ToString() + " , " + iGFOKeyValue.Value.mYCoord.ToString() + " )");
                }

                foreach (KeyValuePair<int, GlobalForeignObject> iGFOKeyValue in mMap.mDistortions)
                {
                    SBMessage.Append("\n Distortion " + iGFOKeyValue.Key.ToString() + ": ( " + iGFOKeyValue.Value.mXCoord.ToString() + " , " + iGFOKeyValue.Value.mYCoord.ToString() + " )");
                }

                foreach (KeyValuePair<int, GlobalForeignObject> iGFOKeyValue in mMap.mCombatDrones)
                {
                    SBMessage.Append("\n Combat Drone " + iGFOKeyValue.Key.ToString() + ": ( " + iGFOKeyValue.Value.mXCoord.ToString() + " , " + iGFOKeyValue.Value.mYCoord.ToString() + " )");
                }

                foreach (KeyValuePair<int, GlobalForeignObject> iGFOKeyValue in mMap.mFactoryDrones)
                {
                    SBMessage.Append("\n Factory Drone " + iGFOKeyValue.Key.ToString() + ": ( " + iGFOKeyValue.Value.mXCoord.ToString() + " , " + iGFOKeyValue.Value.mYCoord.ToString() + " )");
                }

                return SBMessage.ToString();
            }

            /// <summary>
            /// Used for tasks 1 and 2.
            /// </summary>
            /// <returns></returns>
            public report Report()
            {
                return mReportObject;
            }

            public void Reset()
            {
                mScoutThrustControls.ThrustCW = 0;
                mScoutThrustControls.ThrustForward = 0;
                mScoutThrustControls.ThrustRight = 0;

                mScoutActionControls.ShieldOn = false;
                mScoutActionControls.MinerOn = false;
                mScoutActionControls.EnergyExtractorOn = false;

                mMap.ResetMap();
                mNavigation.ResetNavigation();

                mReportObject.X = 0;
                mReportObject.Y = 0;
                mReportObject.complete = false;

                isMinerFinished = false;
                isJavelinActive = false;
            }

            public void think(double aSlowdown = 200, double aAccuracy = 200)
            {
                ScoutThrustControls lTotalThrust = new ScoutThrustControls();
                ScoutThrustControls lWaypointThrust = new ScoutThrustControls();



                //Executes javalin prcedure if drones are in range and a black hole is detected
                if ((mMap.NumberOfObjectsInRange(mMap.mCombatDrones, 1000) > 0 || mMap.NumberOfObjectsInRange(mMap.mCombatDrones, 500) > 0) && mMap.mBlackHoles.Count > 0)
                {
                    int lBHOrbitRange = 450;
                    int lCombatDroneCritRange = 200;
                    int lFactoryDroneCritRange = 150;

                    pose lClosestBH = mMap.GetClosestObject(mMap.mBlackHoles);
                    double lDistancetoBH = mMap.CalculateDistanceFromScout(lClosestBH.X, lClosestBH.Y);





                    //If the scout is in orbit range, wait for enemies to get in critical distance.
                    if (lDistancetoBH < lBHOrbitRange)
                    {
                        mNavigation.ReplaceWaypointAtFront(mMap.CalculateJavelinMidPoint(lClosestBH.X, lClosestBH.Y));
                        ScoutThrustControls lJavThrust = mNavigation.MoveToWaypoint(1, 50);
                        mScoutThrustControls = mNavigation.ApplyPID(lJavThrust, mScoutState);
                        return;


                        //if (isJavelinActive)
                        //{
                        //    mNavigation.ReplaceWaypointAtFront(mMap.CalculateJavelinMidPoint(lClosestBH.X, lClosestBH.Y));
                        //    mScoutThrustControls = mNavigation.MoveToWaypoint(1,50);
                        //    return;
                        //}


                        //If a drone is in critical distance of the scout, then move to the opposite side of the black hole.
                        //else if (mMap.NumberOfObjectsInRange(mMap.mCombatDrones, lCombatDroneCritRange) > 0 || mMap.NumberOfObjectsInRange(mMap.mFactoryDrones, lFactoryDroneCritRange) > 0)
                        //{
                        //    //mNavigation.AddWaypointToFront(mMap.CalculateJavelinEndPoint(lClosestBH.X, lClosestBH.Y));
                        //    //mNavigation.AddWaypointToFront(mMap.CalculateJavelinMidPoint(lClosestBH.X, lClosestBH.Y));
                        //    isJavelinActive = true;
                        //    return;
                        //}


                    }
                    //travel to orbit point

                    pose BHOrbitPoint = mMap.CalculateOrbitPoint(lClosestBH.X, lClosestBH.Y, 400);

                    //Moves to the closest black holes orbit.
                    lWaypointThrust = mNavigation.MoveToTarget(BHOrbitPoint.X, BHOrbitPoint.Y, 100);
                    isJavelinActive = false;



                }

                else
                {
                    isJavelinActive = false;
                    lWaypointThrust = mNavigation.MoveToWaypoint(aSlowdown, aAccuracy);
                }






                //get all of the avoid thrusts
                ScoutThrustControls lBlackHoleAvoidThrust = mNavigation.getObjectsAvoidThrust(mMap.mBlackHoles, 200, 100, 1000);

                ScoutThrustControls lCombatDroneAvoidThrust = mNavigation.getObjectsAvoidThrust(mMap.mCombatDrones);

                ScoutThrustControls lFactoryDroneAvoidThrust = mNavigation.getObjectsAvoidThrust(mMap.mFactoryDrones);

                ScoutThrustControls lAsteroidAvoidThrust = mNavigation.getObjectsAvoidThrust(mMap.mAsteroids, 100, 50, 5);



                //weight everything
                lTotalThrust.ThrustForward += lBlackHoleAvoidThrust.ThrustForward;
                lTotalThrust.ThrustRight += lBlackHoleAvoidThrust.ThrustRight;
                lTotalThrust.ThrustCW += lBlackHoleAvoidThrust.ThrustCW;

                lTotalThrust.ThrustForward += lCombatDroneAvoidThrust.ThrustForward;
                lTotalThrust.ThrustRight += lCombatDroneAvoidThrust.ThrustRight;
                lTotalThrust.ThrustCW += lCombatDroneAvoidThrust.ThrustCW;

                lTotalThrust.ThrustForward += lFactoryDroneAvoidThrust.ThrustForward;
                lTotalThrust.ThrustRight += lFactoryDroneAvoidThrust.ThrustRight;
                lTotalThrust.ThrustCW += lFactoryDroneAvoidThrust.ThrustCW;

                lTotalThrust.ThrustForward += lWaypointThrust.ThrustForward;
                lTotalThrust.ThrustRight += lWaypointThrust.ThrustRight;
                lTotalThrust.ThrustCW += lWaypointThrust.ThrustCW;

                lTotalThrust.ThrustForward += lAsteroidAvoidThrust.ThrustForward;
                lTotalThrust.ThrustRight += lAsteroidAvoidThrust.ThrustRight;
                lTotalThrust.ThrustCW += lAsteroidAvoidThrust.ThrustCW;




                //get single values

                //use PID calcuations and apply them to scout thrust.
                mScoutThrustControls = mNavigation.ApplyPID(lTotalThrust, mScoutState);
            }



        }


        public class Navigation
        {
            Map mMap;

            public PID mXPID = new PID();
            public PID mYPID = new PID();
            public PID mRotationalPID = new PID();

            List<pose> mPath = new List<pose>();

            /// <summary>
            /// Called once at the begining of the game to establish references
            /// </summary>
            /// <param name="aMap"></param>
            /// <param name="aScoutControl"></param>
            /// <param name="aScoutStatus"></param>
            public void Initialize(Map aMap)
            {
                mMap = aMap;
            }

            /// <summary>
            /// Resets Navigation by setting the PID values.
            /// </summary>
            /// <param name="aMap"></param>
            /// <param name="aScoutControl"></param>
            public void ResetNavigation()
            {

                mRotationalPID.Initialize(-3, 3, 10000, 0, 0);
                mXPID.Initialize(-1.5, 1.5, 10, 0, 0);
                mYPID.Initialize(-3, 3, 10, 0, 0);
                mPath.Clear();
            }


            public void CreateSpiralPath()
            {
                for (int i = 0; i < 8; i++)
                {
                    pose lPose = new pose();
                    mPath.Add(lPose);
                }

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

            }

            public void CreateBoxPath()
            {
                for (int i = 0; i < 5; i++)
                {
                    pose lPose = new pose();
                    switch (i)
                    {
                        case 0:
                            lPose.X = 0;
                            lPose.Y = 1000;
                            break;
                        case 1:
                            lPose.X = 1000;
                            lPose.Y = 1000;
                            break;
                        case 2:
                            lPose.X = 1000;
                            lPose.Y = -1000;
                            break;
                        case 3:
                            lPose.X = -1000;
                            lPose.Y = -1000;
                            break;
                        case 4:
                            lPose.X = -1000;
                            lPose.Y = 1000;
                            break;
                    }
                    mPath.Add(lPose);
                }
            }


            public void AddWaypointsToFront(List<pose> aWaypoints)
            {
                for (int i = aWaypoints.Count - 1; i >= 0; i--)
                {
                    AddWaypointToFront(aWaypoints[i]);
                }
            }


            public void AddWaypointToFront(pose aWaypoint)
            {
                mPath.Insert(0, aWaypoint);
            }

            public void ReplaceWaypointAtFront(pose aWaypoint)
            {
                if (mPath.Count < 1)
                    CreateBoxPath();

                mPath[0] = aWaypoint;

            }

            /// <summary>
            /// Moves the scout to a waypoint
            /// </summary>
            /// <param name="aSlowdown">A constant that determines how much the thrust descreases with distance to the target. A larger number slows it more dramatically, enter 1 for no slowdown.</param>
            /// <param name="aAccuracy"></param>
            public ScoutThrustControls MoveToWaypoint(double aSlowdown = 200, double aAccuracy = 100)
            {
                //Makes sure that momevemnt is not attempted until a path exists.
                if (mPath.Count < 2)
                {
                    CreateBoxPath();
                }

                double dist = mMap.CalculateDistanceFromScout(mPath[0].X, mPath[0].Y);

                //if close enough, then go to the next waypoint
                if (dist < aAccuracy)
                {
                    mPath.RemoveAt(0);
                }
                return MoveToTarget(mPath[0].X, mPath[0].Y, aSlowdown);



            }



            /// <summary>
            /// returns a thrust perscription for the given target
            /// </summary>
            /// <param name="targetX">X</param>
            /// <param name="targetY">Y</param>
            /// <param name="aAngularVelSharpness">How sharp the ship is allowed to turn. A lower value restricts the CWVelocity by less.</param>
            /// <param name="aSlowdown">A constant that determines how much the thrust descreases with distance to the target. A larger number slows it more dramatically, enter 1 for no slowdown.</param>
            /// <returns></returns>
            public ScoutThrustControls MoveToTarget(double targetX, double targetY, double aSlowdown = 200, double aAngularVelSharpness = 40)
            {
                const double lMaxVelocity = 0.4;
                ScoutThrustControls lThrustToTarget = new ScoutThrustControls();

                double TwoPI = Math.PI * 2;

                double deltaX = targetX - mMap.mScoutPose.X;
                double deltaY = targetY - mMap.mScoutPose.Y;

                double angleToFace = Math.Atan2(deltaX, deltaY);

                angleToFace = angleToFace % TwoPI;

                double anglebetweenFaceAndCurrentHeading = angleToFace - mMap.mScoutPose.angle;

                anglebetweenFaceAndCurrentHeading = anglebetweenFaceAndCurrentHeading % TwoPI;

                if (anglebetweenFaceAndCurrentHeading < -Math.PI)
                    anglebetweenFaceAndCurrentHeading += TwoPI;
                else if (anglebetweenFaceAndCurrentHeading > Math.PI)
                    anglebetweenFaceAndCurrentHeading -= TwoPI;

                double requiredCWVel = anglebetweenFaceAndCurrentHeading / aAngularVelSharpness;

                if (requiredCWVel > 0.005)
                    requiredCWVel = 0.005;
                else if (requiredCWVel < -0.005)
                    requiredCWVel = -0.005;

                lThrustToTarget.ThrustCW = requiredCWVel;




                double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY); //Distance to waypoint
                double newdX, newdY;
                double requiredXVel, requiredYVel;


                newdX = distance * Math.Sin(anglebetweenFaceAndCurrentHeading); // Get the sideways distance the scout must move
                newdY = distance * Math.Cos(anglebetweenFaceAndCurrentHeading); // Get the forward distance the scout must move

                requiredXVel = newdX / aSlowdown;     // The required velocity is slower as the scout gets closer to the destination
                requiredYVel = newdY / aSlowdown;

                lThrustToTarget.ThrustRight = requiredXVel;

                //mXPID.CalculateThrust(requiredXVel, mScoutState.currentVelocityRight);

                if (requiredYVel > lMaxVelocity)
                    requiredYVel = lMaxVelocity;

                lThrustToTarget.ThrustForward = requiredYVel;

                //mYPID.CalculateThrust(requiredYVel, mScoutState.currentVelocityForward);

                return lThrustToTarget;
            }

            public ScoutThrustControls MoveAwayFromTarget(double targetX, double targetY, double aSpeedUp = 600, double aAngularVelSharpness = 40)
            {




                const double lMaxVelocity = 0.4;
                ScoutThrustControls lThrustToTarget = new ScoutThrustControls();

                double TwoPI = Math.PI * 2;

                //If there is an error in this method, its in these next 4 lines
                pose lAvoidPoint = GetFleePoint(targetX, targetY);

                double deltaX = lAvoidPoint.X - mMap.mScoutPose.X;
                double deltaY = lAvoidPoint.Y - mMap.mScoutPose.Y;

                double angleToFace = Math.Atan2(deltaX, deltaY);

                angleToFace = angleToFace % TwoPI;

                double anglebetweenFaceAndCurrentHeading = angleToFace - mMap.mScoutPose.angle;

                anglebetweenFaceAndCurrentHeading = anglebetweenFaceAndCurrentHeading % TwoPI;

                if (anglebetweenFaceAndCurrentHeading < -Math.PI)
                    anglebetweenFaceAndCurrentHeading += TwoPI;
                else if (anglebetweenFaceAndCurrentHeading > Math.PI)
                    anglebetweenFaceAndCurrentHeading -= TwoPI;

                double requiredCWVel = anglebetweenFaceAndCurrentHeading / aAngularVelSharpness;

                if (requiredCWVel > 0.005)
                    requiredCWVel = 0.005;
                else if (requiredCWVel < -0.005)
                    requiredCWVel = -0.005;

                lThrustToTarget.ThrustCW = requiredCWVel;



                double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY); //Distance to waypoint
                double newdX, newdY;
                double requiredXVel, requiredYVel;


                newdX = distance * Math.Sin(anglebetweenFaceAndCurrentHeading); // Get the sideways distance the scout must move
                newdY = distance * Math.Cos(anglebetweenFaceAndCurrentHeading); // Get the forward distance the scout must move

                requiredXVel = aSpeedUp / newdX;     // The required velocity is faster as the scout gets closer to the destination
                requiredYVel = aSpeedUp / newdY;

                lThrustToTarget.ThrustRight = requiredXVel;

                if (requiredYVel > lMaxVelocity)
                    requiredYVel = lMaxVelocity;

                lThrustToTarget.ThrustForward = requiredYVel;
                return lThrustToTarget;
            }

            public void SetupJavelin()
            {

            }


            /// <summary>
            /// Calculates the thrust needed to avoid any number of black holes close to the scout.
            /// </summary>
            /// <returns>Returns the calculated thrust needed to flee from the black hole.</returns>
            public ScoutThrustControls getObjectsAvoidThrust(Dictionary<int, GlobalForeignObject> aObjects, double aAvoidDistance = 300, double aCriticalDistance = 100, double aSpeedup = 600)
            {
                ScoutThrustControls lTotalAvoidanceThrust = new ScoutThrustControls();
                int lObjectsInRangeCount = 0;

                foreach (KeyValuePair<int, GlobalForeignObject> iAvoidObjectValuePair in aObjects)
                {
                    //Gets the range to the object
                    double lRangeToObject = mMap.CalculateDistanceFromScout(iAvoidObjectValuePair.Value.mXCoord, iAvoidObjectValuePair.Value.mYCoord);
                    double lAngleToObject = mMap.CalculateRelativeAngleFromScout(iAvoidObjectValuePair.Value.mXCoord, iAvoidObjectValuePair.Value.mYCoord);
                    // if scout is close enough, act on avoid.
                    if (lRangeToObject < aAvoidDistance)
                    {
                        lObjectsInRangeCount++;
                        ScoutThrustControls lThrustAwayFromObject = MoveAwayFromTarget(iAvoidObjectValuePair.Value.mXCoord, iAvoidObjectValuePair.Value.mXCoord, aSpeedup);

                        lTotalAvoidanceThrust.ThrustRight += lThrustAwayFromObject.ThrustRight;

                        // If the angle is within the cone or super close act on other thrust
                        if ((lAngleToObject < Math.PI / 3 && lAngleToObject > 0 - Math.PI / 3) || (lAngleToObject > (Math.PI * 2 - Math.PI / 3)) || lRangeToObject < aCriticalDistance)
                        {
                            lTotalAvoidanceThrust.ThrustForward += lThrustAwayFromObject.ThrustForward;
                            lTotalAvoidanceThrust.ThrustCW += lThrustAwayFromObject.ThrustCW;
                        }

                    }

                }
                //get the average thrust for the right thruster
                if (lObjectsInRangeCount != 0)
                    lTotalAvoidanceThrust.ThrustRight = lTotalAvoidanceThrust.ThrustRight / lObjectsInRangeCount;
                return lTotalAvoidanceThrust;
            }


            /// <summary>
            /// Gets the thrust value needed to avoid walls
            /// </summary>
            /// <param name="aAvoidDistance">The distance the scout must be within in order to get an avoidance value. (adjusts side thrust only)</param>
            /// <param name="aDangerDistance">The distance the scout must be within in order to be considered in danger. (adjusts side and forward thrust)</param>
            /// <param name="aCriticalDistance">The distance the scout must be within in order to be considered critical. (adjusts angular thrust) </param>
            /// <param name="aThrustToDistanceStrength">A constant value that determines the thrust output based on the distance from the object. Higher == more thrust</param>
            /// <returns>Avoidance thrust for walls.</returns>
            //private pose getWallAvoidThrust(double aAvoidDistance, double aDangerDistance, double aCriticalDistance, double aThrustToDistanceStrength)
            //{
            //    double lRangeToObject = 0;
            //    double thrust = 0;
            //    pose lTotalAvoidanceThrust = new pose();

            //    double lMaxMapSize = 1500;
            //    double lMinMapSize = -1500;

            //    double lDistanceToPosXWall = lMaxMapSize - mMap.mScoutPose.X;
            //    double lDistanceToNegXWall = -(lMinMapSize - mMap.mScoutPose.X);

            //    double lDistanceToPosYWall = lMaxMapSize - mMap.mScoutPose.Y;
            //    double lDistanceToNegYWall = -(lMinMapSize - mMap.mScoutPose.Y);

            //    //if the scout is within lAvoidDistance of the Pos X wall, take action.
            //    if (lDistanceToPosXWall < aAvoidDistance)
            //    {

            //        thrust += aThrustToDistanceStrength / lDistanceToPosXWall;
            //        double lAngleToWall = mMap.CalculateRelativeAngleFromScout(lMaxMapSize, mMap.mScoutPose.Y);

            //        //adds the flee thrust to the total avoidance thrust
            //        pose lTempAvoidThrust = GetFleePoint(0, thrust, lAngleToWall);
            //        lTotalAvoidanceThrust.X += lTempAvoidThrust.X;

            //        if (lDistanceToPosXWall < aDangerDistance)
            //            lTotalAvoidanceThrust.Y += lTempAvoidThrust.Y;
            //        //else if (lDistanceToPosXWall < aCriticalDistance)
            //            //lTotalAvoidanceThrust = MoveToTarget(lTempAvoidThrust.X, lTempAvoidThrust.Y, 10);

            //    }
            //    //UNFINISHED METHOD
            //    return null;
            //}

            /// <summary>
            /// Gets the flee point from a given point.
            /// </summary>
            /// <param name="aXTarget"></param>
            /// <param name="aYTarget"></param>
            /// <returns></returns>
            private pose GetFleePoint(double aXTarget, double aYTarget)
            {
                double lRange = mMap.CalculateDistanceFromScout(aXTarget, aYTarget);
                double lAngle = mMap.CalculateRelativeAngleFromScout(aXTarget, aYTarget);

                pose lFleePoint = new pose();

                //Relative flee point formula
                lFleePoint.angle = lAngle - Math.PI;
                lFleePoint.X = lRange * Math.Sin(lFleePoint.angle);
                lFleePoint.Y = lRange * Math.Cos(lFleePoint.angle);

                lFleePoint.X += mMap.mScoutPose.X;
                lFleePoint.Y += mMap.mScoutPose.Y;

                /*
                 * The following is the rotation for rotating through an anti-clockwise direction and is found in most text books
                 * X' = xCosA - ySinA
                 * Y' = xSinA + yCosA
                 * 
                 * But we are rotating in a clockwise direction so the rotational equations become;
                 * X' = xCosA + ySinA
                 * Y' = -xCosA + ySinA
                */
                return lFleePoint;
            }

            public ScoutThrustControls ApplyPID(ScoutThrustControls aTargetThrust, ScoutState aCurrentThrust)
            {
                ScoutThrustControls lAdjustedThrust = new ScoutThrustControls();

                lAdjustedThrust.ThrustCW = mRotationalPID.CalculateThrust(aTargetThrust.ThrustCW, aCurrentThrust.currentVelocityAngularCW);
                lAdjustedThrust.ThrustForward = mYPID.CalculateThrust(aTargetThrust.ThrustForward, aCurrentThrust.currentVelocityForward);
                lAdjustedThrust.ThrustRight = mXPID.CalculateThrust(aTargetThrust.ThrustRight, aCurrentThrust.currentVelocityRight);

                return lAdjustedThrust;
            }




        }



        /// <summary>
        /// The class that handles all mapping.
        /// </summary>
        public class Map
        {
            public pose mScoutPose = new pose();

            public Dictionary<int, GlobalForeignObject> mBlackHoles = new Dictionary<int, GlobalForeignObject>();
            public Dictionary<int, GlobalForeignObject> mAsteroids = new Dictionary<int, GlobalForeignObject>();
            public Dictionary<int, GlobalForeignObject> mDistortions = new Dictionary<int, GlobalForeignObject>();
            public Dictionary<int, GlobalForeignObject> mCombatDrones = new Dictionary<int, GlobalForeignObject>();
            public Dictionary<int, GlobalForeignObject> mFactoryDrones = new Dictionary<int, GlobalForeignObject>();

            public const double COLLECTION_RANGE = 70;

            public void ResetMap()
            {
                mScoutPose.X = 0;
                mScoutPose.Y = 0;
                mScoutPose.angle = 0;

                mBlackHoles.Clear();
                mAsteroids.Clear();
                mDistortions.Clear();
                mCombatDrones.Clear();
                mFactoryDrones.Clear();

            }

            /// <summary>
            /// Updates the map every frame.
            /// </summary>
            /// <param name="aRelativeForeignObjects"></param>
            public void UpdateMap(List<RelativeForeignObject> aRelativeForeignObjects)
            {
                foreach (RelativeForeignObject iRelativeForeignObject in aRelativeForeignObjects)
                {
                    switch (iRelativeForeignObject.mTypeOfObject)
                    {
                        case ObjectType.BlackHole:
                            mBlackHoles[iRelativeForeignObject.mObjectID] = GlobalForeignObject.Convert(iRelativeForeignObject, mScoutPose);
                            break;
                        case ObjectType.Asteroid:
                            mAsteroids[iRelativeForeignObject.mObjectID] = GlobalForeignObject.Convert(iRelativeForeignObject, mScoutPose);
                            break;
                        case ObjectType.Distortion:
                            mDistortions[iRelativeForeignObject.mObjectID] = GlobalForeignObject.Convert(iRelativeForeignObject, mScoutPose);
                            break;
                        case ObjectType.CombatDrone:
                            mCombatDrones[iRelativeForeignObject.mObjectID] = GlobalForeignObject.Convert(iRelativeForeignObject, mScoutPose);
                            break;
                        case ObjectType.Factory:
                            mFactoryDrones[iRelativeForeignObject.mObjectID] = GlobalForeignObject.Convert(iRelativeForeignObject, mScoutPose);
                            break;

                    }
                }
            }

            /// <summary>
            /// Calculates a point within collection range of an asteroid field or distortion.
            /// </summary>
            /// <param name="aTargetX"></param>
            /// <param name="aTargetY"></param>
            /// <returns></returns>
            public pose CalculateOrbitPoint(double aTargetX, double aTargetY, double aCollectionRange = COLLECTION_RANGE)
            {
                double lCollecitonDistance = CalculateDistanceFromScout(aTargetX, aTargetY) - aCollectionRange;
                double lCollectionAngle = CalculateRelativeAngleFromScout(aTargetX, aTargetY);

                return CalculateGlobalPosition(lCollecitonDistance, lCollectionAngle);

            }

            public pose CalculateJavelinEndPoint(double aBHX, double aBHY, double aOrbitRange = 200)
            {
                double lDistanceToEndpoint = CalculateDistanceFromScout(aBHX, aBHY) + aOrbitRange;
                double lAngleToEndpoint = CalculateRelativeAngleFromScout(aBHX, aBHY);

                return CalculateGlobalPosition(lDistanceToEndpoint, lAngleToEndpoint);
            }

            public pose CalculateJavelinMidPoint(double aBHX, double aBHY)
            {
                double lDistanceToMidpoint = CalculateDistanceFromScout(aBHX, aBHY) + 50;

                // use arctan to find angle to point

                double lAngleToMidpoint = CalculateRelativeAngleFromScout(aBHX, aBHY) - Math.PI / 2;

                return CalculateGlobalPosition(lDistanceToMidpoint, lAngleToMidpoint);
            }

            public pose CalculateGlobalPosition(double aRangeToTarget, double aRelativeAngleToTarget)
            {
                pose lGlobalPosition = new pose();
                lGlobalPosition.X = aRangeToTarget * Math.Sin(mScoutPose.angle + aRelativeAngleToTarget) + mScoutPose.X;
                lGlobalPosition.Y = aRangeToTarget * Math.Cos(mScoutPose.angle + aRelativeAngleToTarget) + mScoutPose.Y;
                return lGlobalPosition;
            }

            public int NumberOfObjectsInRange(Dictionary<int, GlobalForeignObject> aObjects, double aRange = 200)
            {
                int lDroneCount = 0;
                foreach (KeyValuePair<int, GlobalForeignObject> iObjectKeyValue in aObjects)
                {
                    if (CalculateDistanceFromScout(iObjectKeyValue.Value.mXCoord, iObjectKeyValue.Value.mYCoord) < aRange)
                        lDroneCount++;
                }
                return lDroneCount;
            }

            public pose GetClosestObject(Dictionary<int, GlobalForeignObject> aObjects)
            {
                int lClosetObjectKey = aObjects.Keys.First();

                foreach (KeyValuePair<int, GlobalForeignObject> iObjectKeyValue in aObjects)
                {
                    //If the object in this itteration is closer then the closest object, replace it.
                    if (CalculateDistanceFromScout(iObjectKeyValue.Value.mXCoord, iObjectKeyValue.Value.mYCoord)
                        < CalculateDistanceFromScout(aObjects[lClosetObjectKey].mXCoord, aObjects[lClosetObjectKey].mYCoord))
                    {
                        lClosetObjectKey = iObjectKeyValue.Key;
                    }
                }

                pose lClosetCoords = new pose();
                lClosetCoords.X = aObjects[lClosetObjectKey].mXCoord;
                lClosetCoords.Y = aObjects[lClosetObjectKey].mYCoord;


                return lClosetCoords;
            }




            /// <summary>
            /// Calculates the distance to a target from the Scouts position.
            /// </summary>
            /// <param name="aTargetX"></param>
            /// <param name="aTargetY"></param>
            /// <returns></returns>
            public double CalculateDistanceFromScout(double aTargetX, double aTargetY)
            {
                double deltaX = aTargetX - mScoutPose.X;
                double deltaY = aTargetY - mScoutPose.Y;
                return Math.Sqrt(deltaX * deltaX + deltaY * deltaY); //Distance to waypoint
            }

            public double CalculateGlobalAngleFromScout(double aTargetX, double aTargetY)
            {
                double TwoPI = Math.PI * 2;

                double deltaX = aTargetX - mScoutPose.X;
                double deltaY = aTargetY - mScoutPose.Y;

                double angleToFace = Math.Atan2(deltaX, deltaY);

                return angleToFace % TwoPI;
            }

            /// <summary>
            /// Calculates the distance to a target from the Scouts position.
            /// </summary>
            /// <param name="aTargetX"></param>
            /// <param name="aTargetY"></param>
            /// <returns></returns>
            public double CalculateRelativeAngleFromScout(double aTargetX, double aTargetY)
            {
                double TwoPI = Math.PI * 2;


                double anglebetweenFaceAndCurrentHeading = CalculateGlobalAngleFromScout(aTargetX, aTargetY) - mScoutPose.angle;

                anglebetweenFaceAndCurrentHeading = anglebetweenFaceAndCurrentHeading % TwoPI;

                if (anglebetweenFaceAndCurrentHeading < -Math.PI)
                    anglebetweenFaceAndCurrentHeading += TwoPI;
                else if (anglebetweenFaceAndCurrentHeading > Math.PI)
                    anglebetweenFaceAndCurrentHeading -= TwoPI;

                return anglebetweenFaceAndCurrentHeading;
            }




            /// <summary>
            /// This method was created by the professor. 
            /// </summary>
            /// <param name="aScoutState"></param>
            public void TrackShip(ScoutState aScoutState)             // Use the current velocities to track the ship
            {
                double dx, dy, ndx, ndy;


                dy = aScoutState.averageVelocityForward * aScoutState.deltaTime;  // Distance moved forward since last time (use average velocity)
                dx = aScoutState.averageVelocityRight * aScoutState.deltaTime;    // Distance moved sideways since last time

                mScoutPose.angle += aScoutState.currentVelocityAngularCW * (aScoutState.deltaTime / 2);  // Get half of the angle rotated through since last time
                                                                                                         // (This is the average angle and is used to make the tracking more accurate)
                mScoutPose.angle = mScoutPose.angle % (Math.PI * 2);          // ensure to angle is between -2*PI ans 2*PI

                ndx = dx * Math.Cos(mScoutPose.angle) + dy * Math.Sin(mScoutPose.angle);  // Rotate the forward and sideways distances to world coords
                ndy = -dx * Math.Sin(mScoutPose.angle) + dy * Math.Cos(mScoutPose.angle);
                mScoutPose.X += ndx;                             // Update the scouts coordinates
                mScoutPose.Y += ndy;

                mScoutPose.angle += aScoutState.currentVelocityAngularCW * (aScoutState.deltaTime / 2);    // Get the last half of the angle rotated through
                mScoutPose.angle = mScoutPose.angle % (Math.PI * 2);          // ensure to angle is between -2*PI ans 2*PI

            }
        }


        /// <summary>
        /// An object in space that is represented in a global mapping.
        /// </summary>
        public class GlobalForeignObject
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
            public static GlobalForeignObject Convert(RelativeForeignObject aRFO, pose aScoutPose)
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








        public class PID
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
            public void Initialize(double aMinOut, double aMaxOut, double aProportionalConstant, double aIntegralConstant, double aDerivativeConstant)
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
    }
}