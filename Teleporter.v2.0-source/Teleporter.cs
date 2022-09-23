using System;
using System.Collections.Generic;
using UnityEngine;
using SFS;
using SFS.World;
using SFS.WorldBase;
using static SFS.Base;
using System.Globalization;
using SFS.UI;

namespace RocketTeleporter
{
    class Teleporter : MonoBehaviour
    {
        Dictionary<string, Planet> planets = planetLoader.planets;
        List<string> planetNames = new List<string>();
        Vector2 scrollVec = Vector2.zero;
        private void Start()
        {
            foreach (string planetName in planets.Keys)
            {
                this.planetNames.Add(planetName);
            };
        }

        public static Rocket currentRocket;
        public static SFS.Variables.Planet_Local currentPlanet;
        public static double posX, posY, velX, velY, angle, tryparse, height, locAngle, velAng;
        public static double xPosDisplay, yPosDisplay, xVelDisplay, yVelDisplay, angleDisplay, heightDispaly, locAngleDisplay, angVelDisplay;
        public bool pause, unsafeMode = false, values, valuesPrev, planet, loc, locPrev, orbit;
        int selectedPlanetIndex;

        string xPosStr, yPosStr, xVelStr, yVelStr, angleStr, heightStr, locAngleStr, angVelStr;

        public static Rect mainWindowRect = new Rect(100f, 100f, 180f, 170f);
        public static Rect valuesWindowRect = new Rect(100f, 270f, 180f, 430f);
        public static Rect planetWindowRect = new Rect(280, 310, 200, 230);
        public static Rect locWindowRect = new Rect(280,100, 180, 210);
        public static Rect orbitWindowRect = new Rect(280, 230, 200, 230);


        public double mod(double a, double b)
        {
            return a - b * Math.Floor(a / b);
        }
        public Double2 LocToPosition(Double angle, Double height)
        {
            return Double2.CosSin((0.017453292 * angle)) * height;
        }
        public void Pause(bool pause)
        {
            if (pause)
                MsgDrawer.main.Log("Pause function currently not working, sorry");
        }
        private void ResetValues()
        {
            xPosStr = xPosDisplay.ToString();
            yPosStr = yPosDisplay.ToString();
            xVelStr = xVelDisplay.ToString();
            yVelStr = yVelDisplay.ToString();
            angleStr = angleDisplay.ToString();
            //heightStr = heightDispaly.ToString();
            //locAngleStr = locAngleDisplay.ToString();
            angVelStr = angVelDisplay.ToString();
        }
        private void ResetLoc()
        {
            heightStr = heightDispaly.ToString();
            locAngleStr = locAngleDisplay.ToString();
        }
        public void ChangePlanet()
        {
            string planetName = planetNames[selectedPlanetIndex];
            Planet planet = planets[planetName];

            SFS.Variables.Planet_Local planet_local = new SFS.Variables.Planet_Local();
            planet_local.Value = planet;
            currentRocket.location.planet = planet_local;
            currentRocket.rb2d.transform.position = WorldView.ToLocalPosition(new Double2(0, planet_local.Value.Radius + planet_local.Value.TimewarpRadius_Descend));
        }
        public void SetValues()
        {
            currentRocket.rb2d.transform.position = WorldView.ToLocalPosition(new Double2(posX, posY));
            currentRocket.rb2d.velocity = WorldView.ToLocalVelocity(new Double2(velX, velY));
            currentRocket.rb2d.transform.rotation = Quaternion.Euler(0, 0, (float)-angle);
            currentRocket.rb2d.angularVelocity = (float)-velAng;
            //currentRocket.rb2d.AddTorque(currentRocket.rb2d.inertia);
        }
        public void SetLocation()
        {
            currentRocket.rb2d.transform.position = WorldView.ToLocalPosition(LocToPosition(NormalizeAngle(locAngle+90), height + currentPlanet.Value.Radius));
            currentRocket.rb2d.velocity = WorldView.ToLocalVelocity(Double2.zero);
        }
        public double NormalizeAngle(double angle)
        {
            angle = angle + 180;
            angle = mod(angle, 360);
            return angle - 180;
        }

        public void MainWindowFunc(int windowID)
        {
            values = GUI.Toggle(new Rect(10f, 20f, 160f, 20f), values, "Teleporter");
            loc = GUI.Toggle(new Rect(10f, 40f, 160f, 20f), loc, "Set Location");
            planet = GUI.Toggle(new Rect(10f, 60f, 160f, 20f), planet, "Change Planet");
            orbit = GUI.Toggle(new Rect(10f, 80f, 160f, 20f), orbit, "Set Orbit");
            if (valuesPrev != values) ResetValues();
            if (locPrev != loc) ResetLoc();
            valuesPrev = values;
            locPrev = loc;
            pause = GUI.Toggle(new Rect(10f, 120f, 160f, 20f), pause, "Pause");
            unsafeMode = GUI.Toggle(new Rect(10f, 140f, 160f, 20f), unsafeMode, "Unsafe Mode");
            Pause(pause);
            GUI.DragWindow();
        }
        public void ValuesWindowFunc(int windowID)
        {
            GUI.Label(new Rect(10f, 20f, 160f, 20f), "Position");
            GUI.Label(new Rect(10f, 40f, 160f, 20f), "X: " + xPosDisplay.ToString());
            xPosStr = GUI.TextField(new Rect(10f, 60f, 160f, 20f), xPosStr);
            GUI.Label(new Rect(10f, 80f, 160f, 20f), "Y: " + yPosDisplay.ToString());
            yPosStr = GUI.TextField(new Rect(10f, 100f, 160f, 20f), yPosStr);

            GUI.Label(new Rect(10f, 130f, 160f, 20f), "Velocity");
            GUI.Label(new Rect(10f, 150f, 160f, 20f), "X: " + xVelDisplay.ToString());
            xVelStr = GUI.TextField(new Rect(10f, 170f, 160f, 20f), xVelStr);
            GUI.Label(new Rect(10f, 190f, 160f, 20f), "Y: " + yVelDisplay.ToString());
            yVelStr = GUI.TextField(new Rect(10f, 210f, 160f, 20f), yVelStr);
            GUI.Label(new Rect(10f, 230f, 160f, 40f), "Angular: \n" + angVelDisplay.ToString());
            angVelStr = GUI.TextField(new Rect(10f, 265f, 160f, 20f), angVelStr);

            GUI.Label(new Rect(10f, 295f, 160f, 40f), "Angle: \n" + angleDisplay.ToString());
            angleStr = GUI.TextField(new Rect(10f, 330f, 160f, 20f), angleStr);

            if (GUI.Button(new Rect(30f, 360f, 120f, 20f), "Reset")) ResetValues();

            if (GUI.Button(new Rect(10f, 390f, 160f, 30f), "Submit"))
            {
                if (Double.TryParse(xPosStr, out tryparse) & Double.TryParse(yPosStr, out tryparse) & Double.TryParse(xVelStr, out tryparse) & Double.TryParse(yVelStr, out tryparse) & Double.TryParse(angleStr, out tryparse) & Double.TryParse(angVelStr, out velAng))
                {
                    posX = double.Parse(xPosStr);
                    posY = double.Parse(yPosStr);
                    velX = double.Parse(xVelStr);
                    velY = double.Parse(yVelStr);
                    angle = double.Parse(angleStr);
                    velAng = double.Parse(angVelStr);
                }
                else
                {
                    Debug.Log("[Teleporter] Failed to parse a string!");
                }

                if (!unsafeMode)
                {
                    if (new Double2(posX, posY).magnitude < currentPlanet.Value.Radius) Debug.Log("[Teleporter] Cannot teleport below ground (Enable \"Unsafe mofe\" to do so)");
                    else if (new Double2(posX, posY).magnitude > currentPlanet.Value.SOI) Debug.Log("[Teleporter] Cannot teleport outside SOI (Enable \"Unsafe mofe\" to do so)");

                    else
                    {
                        SetValues();
                    }
                }
                else
                {
                    SetValues();
                }
            }
            GUI.DragWindow();
        }
        public void PlanetWindowFunc(int windowID)
        {
            GUI.Label(new Rect(2, 20, 160, 50), "Select a planet from the list below, then press the \"Teleport to\" button to teleport.");
            if (GUI.Button(new Rect(2, 70, 160, 20), "Teleport to " + this.planetNames[selectedPlanetIndex]))
            {
                ChangePlanet();
            }
            this.scrollVec = GUI.BeginScrollView(new Rect(2, 95, 196, 130), this.scrollVec, new Rect(0, 75, 196, planets.Count * 15), false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
            selectedPlanetIndex = GUI.SelectionGrid(new Rect(0, 75, 176, planets.Count * 15), selectedPlanetIndex, this.planetNames.ToArray(), 2);
            GUI.EndScrollView();
            //if (GUI.Button(new Rect(2, 230, 196, 20), "Close Change Planet Menu")) planet = false;
            GUI.DragWindow();
        }
        public void LocWindowFunc(int windowID)
        {
            GUI.Label(new Rect(10f, 20f, 160f, 40f), "Height: \n" + heightDispaly.ToString());
            heightStr = GUI.TextField(new Rect(10f, 55f, 160f, 20f), heightStr);
            GUI.Label(new Rect(10f, 75f, 160f, 40f), "Angle: \n" + locAngleDisplay.ToString());
            locAngleStr = GUI.TextField(new Rect(10f, 110f, 160f, 20f), locAngleStr);

            if (GUI.Button(new Rect(30f, 140f, 120f, 20f), "Reset")) ResetLoc();

            if (GUI.Button(new Rect(10f, 170f, 160f, 30f), "Submit"))
            {
                if (Double.TryParse(heightStr, out tryparse) & Double.TryParse(locAngleStr, out tryparse))
                {
                    height = double.Parse(heightStr);
                    locAngle = -double.Parse(locAngleStr);
                    
                }
                else
                {
                    Debug.Log("[Teleporter] Failed to parse a string!");
                }

                if (!unsafeMode)
                {
                    if (height + currentPlanet.Value.Radius < currentPlanet.Value.Radius) Debug.Log("[Teleporter] Cannot teleport below ground (Enable \"Unsafe mofe\" to do so)");
                    else if (height + currentPlanet.Value.Radius > currentPlanet.Value.SOI) Debug.Log("[Teleporter] Cannot teleport outside SOI (Enable \"Unsafe mofe\" to do so)");

                    else
                    {
                        SetLocation();
                    }
                }
                else
                {
                    SetLocation();
                }
            }
            GUI.DragWindow();
        }
        public void OrbitWindowFunc(int windowID)
        {
            GUI.Label(new Rect(10f, 20f, 160f, 40f), "Coming Soon!");
        }

        public void OnGUI()
        {
            if (VideoSettingsPC.main.uiOpacitySlider.value == 0) return;
            if (PlayerController.main.player.Value == null)
            {
                currentRocket = null;
                return;
            }
            currentRocket = (PlayerController.main.player.Value as Rocket);
            currentPlanet = currentRocket.location.planet;

            xPosDisplay = WorldView.ToGlobalPosition(currentRocket.rb2d.position).x;
            yPosDisplay = WorldView.ToGlobalPosition(currentRocket.rb2d.position).y;

            xVelDisplay = currentRocket.location.velocity.Value.x;
            yVelDisplay = currentRocket.location.velocity.Value.y;
            angVelDisplay = -currentRocket.rb2d.angularVelocity;

            //angleDisplay = -currentRocket.rb2d.rotation;
            angleDisplay = -NormalizeAngle(currentRocket.rb2d.transform.eulerAngles.z);

            heightDispaly = Math.Sqrt(xPosDisplay * xPosDisplay + yPosDisplay * yPosDisplay) - currentPlanet.Value.Radius;
            locAngleDisplay = -NormalizeAngle(currentRocket.location.position.Value.AngleDegrees - 90);

            mainWindowRect = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), mainWindowRect, new GUI.WindowFunction(MainWindowFunc), "Teleporter by Osmo");
            if (values) valuesWindowRect = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), valuesWindowRect, new GUI.WindowFunction(ValuesWindowFunc), "Teleport");
            if (planet) planetWindowRect = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), planetWindowRect, new GUI.WindowFunction(PlanetWindowFunc), "Change Planet");
            if (loc) locWindowRect = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), locWindowRect, new GUI.WindowFunction(LocWindowFunc), "Set location");
            if (orbit) orbitWindowRect = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), orbitWindowRect, new GUI.WindowFunction(OrbitWindowFunc), "Set Orbit");

        }
    }

}
