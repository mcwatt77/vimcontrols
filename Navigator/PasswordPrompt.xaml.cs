namespace Navigator
{
    /// <summary>
    /// Interaction logic for PasswordPrompt.xaml
    /// </summary>
    public partial class PasswordPrompt
    {
        public PasswordPrompt()
        {
            InitializeComponent();
        }

        public static string RetrievePassword()
        {
            var prompt = new PasswordPrompt();
            prompt.ShowDialog();
            return prompt.textBox1.Password;
        }
    }
}
