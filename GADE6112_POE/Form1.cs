using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GADE6112_POE
{
    //Ryan Kennedy
    //19013266
    //Task 3

    //Enum for telling the teams apart
    public enum Faction
    {
        Dire,
        Radient,
        Neutral
    }

    //Enum for the different resource types
    public enum ResourceType
    {
        Gold,
        Iron
    }

    public partial class Form1 : Form
    {
        //Variables that can be adjusted by the user to change the map size
        int mapHeight = 20;
        int mapWidth = 20;

        //Variables to indacate how many resources each team has
        int direResources = 0;
        int radientResources = 0;

        //Buttons to represent the map
        //Button[,] buttons;

        //Variables to adjust how many units and buildings will spawn
        static int unitNum = 8;
        static int buildingNum = 6;

        //Map object
        Map m; 

        public Form1()
        {
            InitializeComponent();
        }

        //Runs when the form loads to set up the map
        private void Form1_Load(object sender, EventArgs e)
        {
            m = new Map(unitNum, buildingNum, 20, 20);

            //buttons = new Button[20, 20];

            m.GenerateBattlefeild();
            Placebuttons();
        }

        //Places the buttons on the form and puts the units in the buttons 
        public void Placebuttons()
        {
            gbMap.Controls.Clear();

            Size btnSize = new Size(30, 30);

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (m.map[x, y] == "R")//If the string map representation has an 'R' for a ranged unit
                    {
                        Button btn = new Button();

                        btn.Size = btnSize;
                        btn.Location = new Point(x * 30, y * 30);

                        if (m.unitMap[x,y] is RangedUnit)
                        {
                            RangedUnit R = (RangedUnit)m.unitMap[x, y];
                            btn.Text = R.Symbol;
                            if (R.FactionType == Faction.Dire)
                            {
                                btn.BackColor = Color.Red;
                            }
                            else if(R.FactionType == Faction.Radient)
                            {
                                btn.BackColor = Color.Green;
                            }
                            btn.Name = m.unitMap[x, y].ToString();
                            btn.Click += MyButtonClick;
                            gbMap.Controls.Add(btn);
                        }
                    }
                    else if (m.map[x, y] == "M")//If the string map representation has an 'M' for a melee unit
                    {
                        Button btn = new Button();

                        btn.Size = btnSize;
                        btn.Location = new Point(x * 30, y * 30);

                        if (m.unitMap[x, y] is MeleeUnit)
                        {
                            MeleeUnit M = (MeleeUnit)m.unitMap[x, y];
                            btn.Text = M.Symbol;
                            if (M.FactionType == Faction.Dire)
                            {
                                btn.BackColor = Color.Red;
                            }
                            else if(M.FactionType == Faction.Radient)
                            {
                                btn.BackColor = Color.Green;
                            }
                            btn.Name = m.unitMap[x, y].ToString();
                            btn.Click += MyButtonClick;
                            gbMap.Controls.Add(btn);
                        }
                    }
                    else if (m.map[x,y] == "W")//If the string map representation has an 'W' for a wizard unit
                    {
                        Button btn = new Button();

                        btn.Size = btnSize;
                        btn.Location = new Point(x * 30, y * 30);

                        if (m.unitMap[x, y] is WizardUnit)
                        {
                            WizardUnit W = (WizardUnit)m.unitMap[x, y];
                            btn.Text = W.Symbol;

                            btn.BackColor = Color.Turquoise;

                            btn.Name = m.unitMap[x, y].ToString();
                            btn.Click += MyButtonClick;
                            gbMap.Controls.Add(btn);
                        }
                    }
                    else if (m.map[x,y] == "FB")//If the string map representation has an 'FB' for a factory building
                    {
                        Button btn = new Button();

                        btn.Size = btnSize;
                        btn.Location = new Point(x * 30, y * 30);

                        if (m.buildingMap[x, y] is FactoryBuilding)
                        {
                            FactoryBuilding FB = (FactoryBuilding)m.buildingMap[x, y];
                            btn.Text = FB.Symbol;
                            if (FB.FactionType == Faction.Dire)
                            {
                                btn.BackColor = Color.Red;
                            }
                            else if(FB.FactionType == Faction.Radient)
                            {
                                btn.BackColor = Color.Green;
                            }
                            btn.Name = m.buildingMap[x, y].ToString();
                            btn.Click += MyButtonClick;
                            gbMap.Controls.Add(btn);
                        }
                    }
                    else if (m.map[x,y] == "RB")//If the string map representation has an 'RB' for a resource building
                    {
                        Button btn = new Button();

                        btn.Size = btnSize;
                        btn.Location = new Point(x * 30, y * 30);

                        if (m.buildingMap[x, y] is ResourceBuilding)
                        {
                            ResourceBuilding RB = (ResourceBuilding)m.buildingMap[x, y];
                            btn.Text = RB.Symbol;
                            if (RB.FactionType == Faction.Dire)
                            {
                                btn.BackColor = Color.Red;
                            }
                            else if(RB.FactionType == Faction.Radient)
                            {
                                btn.BackColor = Color.Green;
                            }
                            btn.Name = m.buildingMap[x, y].ToString();
                            btn.Click += MyButtonClick;
                            gbMap.Controls.Add(btn);
                        }
                    }
                }
            }
        }

        //Starts the timer when the button is clicked
        private void btnStart_Click(object sender, EventArgs e)
        {
            GameTick.Enabled = true;

            btnSetSize.Enabled = false;
        }

        //Pauses the timer when the button is clicked
        private void btnPause_Click(object sender, EventArgs e)
        {
            GameTick.Enabled = false;
        }

        //Timer to run every second
        private void GameTick_Tick(object sender, EventArgs e)
        {
            GameLogic();

            lblDResources.Text = "Dire Resources: " + direResources;
            lblRResources.Text = "Radient Resources: " + radientResources;
            lblRound.Text = "Round: " + m.round; 
        }

        //Runs all the logic behind the game
        public void GameLogic()
        {

            //Working out if both teams are alive
            int dire = 0;
            int radiant = 0;

            foreach (ResourceBuilding RB in m.mines)
            {
                if (RB.FactionType == Faction.Dire)
                {
                    dire++;
                }
                else
                {
                    radiant++;
                }
            }

            foreach (FactoryBuilding FB in m.factories)
            {
                if (FB.FactionType == Faction.Dire)
                {
                    dire++;
                }
                else
                {
                    radiant++;
                }
            }

            foreach (MeleeUnit u in m.meleeUnits)
            {
                if(u.FactionType == Faction.Dire)
                {
                    dire++;
                }
                else
                {
                    radiant++;
                }
            }

            foreach (RangedUnit u in m.rangedUnits)
            {
                if (u.FactionType == Faction.Dire)
                {
                    dire++;
                }
                else
                {
                    radiant++;
                }
            }

            if (dire > 0 && radiant > 0)//Checks to see if both teams are still alive
            {
                foreach (ResourceBuilding RB in m.mines)
                {
                    if(RB.FactionType == Faction.Dire)
                    {
                        direResources += RB.GenerateResource();
                    }
                    else if(RB.FactionType == Faction.Radient)
                    {
                        radientResources += RB.GenerateResource();
                    }
                }

                foreach (FactoryBuilding FB in m.factories)
                {
                    Unit u = FB.SpawnUnit();

                    if (FB.FactionType == Faction.Dire && direResources > FB.SpawnCost)
                    {
                        if (m.round % FB.SpawnSpeed == 0)
                        {
                            m.units.Add(u);

                            if (u is MeleeUnit)
                            {
                                MeleeUnit M = (MeleeUnit)u;

                                M.MapHeight = mapHeight;
                                M.MapWidth = mapWidth;
                                m.meleeUnits.Add(M);
                            }
                            else if (u is RangedUnit)
                            {
                                RangedUnit R = (RangedUnit)u;

                                R.MapHeight = mapHeight;
                                R.MapWidth = mapWidth;
                                m.rangedUnits.Add(R);
                            }
                            direResources -= FB.SpawnCost;
                        }
                    }
                    else if (FB.FactionType == Faction.Radient && radientResources > FB.SpawnCost)
                    {
                        if (m.round % FB.SpawnSpeed == 0)
                        {
                            m.units.Add(u);

                            if (u is MeleeUnit)
                            {
                                MeleeUnit M = (MeleeUnit)u;

                                m.meleeUnits.Add(M);
                            }
                            else if (u is RangedUnit)
                            {
                                RangedUnit R = (RangedUnit)u;

                                m.rangedUnits.Add(R);
                            }
                            radientResources -= FB.SpawnCost;
                        }
                    }
                }

                foreach (Unit u in m.units)
                {
                    u.CheckAttackRange(m.units, m.buildings);
                }

                m.round++;
                m.PlaceUnits();
                m.PlaceBuildings();
                Placebuttons();


            }
            else
            {
                m.PlaceUnits();
                m.PlaceBuildings();
                Placebuttons();
                GameTick.Enabled = false;

                if (dire > radiant)
                {
                    MessageBox.Show("Dire Wins in " + m.round + " rounds");
                }
                else
                {
                    MessageBox.Show("Radiant Wins in " + m.round + " rounds");
                }
            }

            //Checks to see who has died and needs to be deleted
            for (int i = 0; i < m.rangedUnits.Count; i++)
            {
                if (m.rangedUnits[i].Death())
                {
                    m.map[m.rangedUnits[i].PosX, m.rangedUnits[i].PosY] = "";
                    m.rangedUnits.RemoveAt(i);
                    
                }
            }

            for (int i = 0; i < m.meleeUnits.Count; i++)
            {
                if (m.meleeUnits[i].Death())
                {
                    m.map[m.meleeUnits[i].PosX, m.meleeUnits[i].PosY] = "";
                    m.meleeUnits.RemoveAt(i);

                }
            }

            for (int i = 0; i < m.wizardUnits.Count; i++)
            {
                if (m.wizardUnits[i].Death())
                {
                    m.map[m.wizardUnits[i].PosX, m.wizardUnits[i].PosY] = "";
                    m.wizardUnits.RemoveAt(i);

                }
            }

            for (int i = 0; i < m.units.Count; i++)
            {
                if (m.units[i].Death())
                {
                    if(m.units[i] is MeleeUnit)
                    {
                        MeleeUnit M = (MeleeUnit)m.units[i];
                        m.map[M.PosX, M.PosY] = "";
                    }
                    else if (m.units[i] is RangedUnit)
                    {
                        RangedUnit R = (RangedUnit)m.units[i];
                        m.map[R.PosX, R.PosY] = "";
                    }
                    else if (m.units[i] is WizardUnit)
                    {
                        WizardUnit W = (WizardUnit)m.units[i];
                        m.map[W.PosX, W.PosY] = "";
                    }

                    m.units.RemoveAt(i);
                }
            }

            for (int i = 0; i < m.factories.Count; i++)
            {
                if (m.factories[i].Death())
                {
                    m.map[m.factories[i].PosX, m.factories[i].PosY] = "";
                    m.factories.RemoveAt(i);
                }
            }

            for (int i = 0; i < m.mines.Count; i++)
            {
                if (m.mines[i].Death())
                {
                    m.map[m.mines[i].PosX, m.mines[i].PosY] = "";
                    m.mines.RemoveAt(i);
                }
            }

            for (int i = 0; i < m.buildings.Count; i++)
            {
                if (m.buildings[i].Death())
                {
                    if (m.buildings[i] is FactoryBuilding)
                    {
                        FactoryBuilding FB = (FactoryBuilding)m.buildings[i];
                        m.map[FB.PosX, FB.PosY] = "";
                    }
                    else if (m.buildings[i] is ResourceBuilding)
                    {
                        ResourceBuilding RB = (ResourceBuilding)m.buildings[i];
                        m.map[RB.PosX, RB.PosY] = "";
                    }

                    m.buildings.RemoveAt(i);
                }
            }


        }

        //The on click event of the buttons with the units
        public void MyButtonClick(object sender, EventArgs e)
        {
            Button btn = ((Button)sender);

            foreach (Unit u in m.units)
            {
                if(btn.Name == u.ToString())
                {
                    txtOutput.Text = u.ToString();
                }
            }

            foreach (Building b in m.buildings)
            {
                if (btn.Name == b.ToString())
                {
                    txtOutput.Text = b.ToString();
                }
            }
        }

        //Saves the game state when the button is clicked
        private void btnSave_Click(object sender, EventArgs e)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream("Save.dat", FileMode.Create, FileAccess.Write, FileShare.None);

            try
            {
                using (fs)
                {
                    bf.Serialize(fs, m);
                }

                MessageBox.Show("Save successful");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //Loads the saved game state when the button is clicked
        private void btnRead_Click(object sender, EventArgs e)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream("Save.dat", FileMode.Open, FileAccess.Read, FileShare.None);

            try
            {
                using (fs)
                {
                    Map mp = (Map)bf.Deserialize(fs);
                    m = mp;
                }

                mapHeight = m.mapHeight;
                mapWidth = m.mapWidth;

                Placebuttons();
                lblRound.Text = "Round: " + m.round;


                MessageBox.Show("Loading successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnSetSize_Click(object sender, EventArgs e)
        {
            try
            {
                mapHeight = Convert.ToInt32(txtHeight.Text);
                mapWidth = Convert.ToInt32(txtWidth.Text);

                if (mapHeight < 10 || mapWidth < 10)
                {
                    MessageBox.Show("Please enter a larger area than 9x9.");
                }
                else
                {
                    m = new Map(unitNum, buildingNum, mapHeight, mapWidth);

                    //buttons = new Button[mapWidth, mapHeight];

                    m.GenerateBattlefeild();
                    Placebuttons();
                }
            }
            catch
            {
                MessageBox.Show("Please enter a valid number.");
            }
        }
    }
}
