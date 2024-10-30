using KeyAuth;

namespace Loader
{
    public partial class Form1 : Form
    {
        public static api KeyAuthApp = new api(
        name: "App name", // App name - VaultCord.com FREE Discord backup bot for members & your entire server saved from terms and nukes!
        ownerid: "account id", // Account ID
        version: "1.0" // Application version. Used for automatic downloads see video here https://www.youtube.com/watch?v=kW195PLCBKs
                       //path: @"Your_Path_Here" // (OPTIONAL) see tutorial here https://www.youtube.com/watch?v=I9rxt821gMk&t=1s
        );

        static string random_string()
        {
            string str = null;

            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                str += Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))).ToString();
            }
            return str;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            KeyAuthApp.init();

            if (!KeyAuthApp.response.success)
            {
                MessageBox.Show(KeyAuthApp.response.message);
                Environment.Exit(0);
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            KeyAuthApp.license(keyBox.Text);
            if (KeyAuthApp.response.success)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Status: " + KeyAuthApp.response.message);
            }
            
        }

        private void keyBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
