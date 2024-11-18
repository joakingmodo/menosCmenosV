using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinForms = System.Windows.Forms;


namespace menosCmenosV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MenosCMenosV : Window
    {
        public string arquivoFBK;

        public string caminhoFire = "";

        public string batFilePath;

        public string arquivoFDB;

        public string pastaDestino;

        public string outputFileName2;

        public string outputFileName1;

        public string textBoxContent1;
        public string textBoxContent2;

        public string arquivoFDBsolo;

        public string arquivoFBKsolo;

        public string itemSelecionado;

        public string[] caminhos { get; set; }
        public MenosCMenosV()
        {
            InitializeComponent();

            caminhos = new string[] { "Fire 2.5 32 bits", "Fire 2.5 64 bits", "Fire 4.0 32 bits", "Fire 4.0 64 bits" };

            DataContext = this;

            meuComboBox.SelectionChanged += ComboBox_SelectionChanged;

            
            


        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            itemSelecionado = meuComboBox.SelectedItem as string; // Get selected item

            if (itemSelecionado == null)
            {
                System.Windows.MessageBox.Show("Por favor, selecione a pasta do GBAK do Firebird.");
                return;
            }

            switch (itemSelecionado)
            {
                case "Fire 2.5 32 bits":
                    caminhoFire = @"C:\\Program Files (x86)\\Firebird\\Firebird_2_5\\bin";
                    if (!System.IO.File.Exists(caminhoFire + "\\gbak.exe"))
                    {
                        System.Windows.MessageBox.Show("Erro: O arquivo 'gbak' não foi encontrado. Verifique se o caminho está correto.");
                        return;
                    }
                    break;
                case "Fire 2.5 64 bits":
                    caminhoFire = @"C:\\Program Files\\Firebird\\Firebird_2_5\\bin";
                    if (!System.IO.File.Exists(caminhoFire + "\\gbak.exe"))
                    {
                        System.Windows.MessageBox.Show("Erro: O arquivo 'gbak' não foi encontrado. Verifique se o caminho está correto.");
                        return;
                    }
                    break;
                case "Fire 4.0 32 bits":
                    caminhoFire = @"C:\\Program Files (x86)\\Firebird\\Firebird_4_0";
                    if (!System.IO.File.Exists(caminhoFire + "\\gbak.exe"))
                    {
                        System.Windows.MessageBox.Show("Erro: O arquivo 'gbak' não foi encontrado. Verifique se o caminho está correto.");
                        return;
                    }
                    break;
                case "Fire 4.0 64 bits":
                    caminhoFire = @"C:\\Program Files\\Firebird\\Firebird_4_0";
                    if (!System.IO.File.Exists(caminhoFire + "\\gbak.exe"))
                    {
                        System.Windows.MessageBox.Show("Erro: O arquivo 'gbak' não foi encontrado. Verifique se o caminho está correto.");
                        return;
                    }
                    break;
            }

        }


        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            outputFileNameTextBox.Focus();
            Keyboard.ClearFocus();

            // Agora o valor deve estar atualizado
            textBoxContent1 = outputFileNameTextBox.Text;

            // Abrir o OpenFileDialog para selecionar o arquivo FBK
            WinForms.OpenFileDialog openFileDialog = new WinForms.OpenFileDialog
            {
                Filter = "Arquivos FBK (*.fbk)|*.fbk"
            };
            if (openFileDialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                arquivoFBK = openFileDialog.FileName;

                arquivoFBKsolo = openFileDialog.SafeFileName;

                fbk.Text = arquivoFBK;

                // Open FolderBrowserDialog to select destination folder

            }

        }


        private void Button1_1_Click(object sender, RoutedEventArgs e)
        {
            outputFileNameTextBox.Focus();
            Keyboard.ClearFocus();

            // Agora o valor deve estar atualizado
            textBoxContent1 = outputFileNameTextBox.Text;

            outputFileName1 = outputFileNameTextBox.Text.Trim();

            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == WinForms.DialogResult.OK)
                {
                    string pastaDestino = folderBrowserDialog.SelectedPath;

                    if (string.IsNullOrEmpty(textBoxContent1))
                    {
                        System.Windows.MessageBox.Show("Por favor, insira um nome para o arquivo de saída.");
                        return;
                    }

                    // Create the full output path
                    string outputPath = System.IO.Path.Combine(pastaDestino, outputFileName1 + ".fdb");

                    destinofdb.Text = outputPath;

                    string comando = $"cd /d \"{caminhoFire}\" && gbak -c -v -user sysdba -pass masterkey \"{arquivoFBK}\" \"{outputPath}\"";

                    // Caminho temporário para o script .bat
                    batFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "RunAsAdmin.bat");

                    // Escreve o comando no arquivo .bat, incluindo "pause" para manter o console aberto em caso de erro
                    System.IO.File.WriteAllText(batFilePath, comando + " || pause");
                }
            }
        }

        private void Button1_2_Click(object sender, RoutedEventArgs e)
        {

            if (itemSelecionado == null)
            {
                System.Windows.MessageBox.Show("Por favor, selecione a pasta do GBAK do Firebird.");
                return;
            }

            outputFileNameTextBox.Focus();
            Keyboard.ClearFocus();

            // Agora o valor deve estar atualizado
            textBoxContent1 = outputFileNameTextBox.Text;

            if (string.IsNullOrEmpty(textBoxContent1))
            {
                System.Windows.MessageBox.Show("Por favor, insira um nome para o arquivo de saída.");
                return;
            }

 
            if (string.IsNullOrEmpty(arquivoFBK))
            {
                System.Windows.MessageBox.Show("Não te faz de louco, coloca o arquivo FBK.");
                return;
            }

            if (string.IsNullOrEmpty(pastaDestino))
            {
                System.Windows.MessageBox.Show("Não te faz de louco, coloca a pasta de destino.");
                return;
            }

            using (Process process = new Process())
            {
                process.StartInfo.FileName = batFilePath;
                process.StartInfo.Verb = "runas";  // Executar como administrador
                process.StartInfo.UseShellExecute = true; // Necessário para usar o Verb

                try
                {
                    process.Start();
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        System.Windows.MessageBox.Show("Conversão realizada com sucesso!");

                        gerarlogfdb(arquivoFBKsolo, pastaDestino, outputFileName1);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Ocorreu um erro durante a conversão.");
                    }
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    System.Windows.MessageBox.Show("A operação foi cancelada ou ocorreu um erro ao tentar obter privilégios administrativos.");
                }
                finally
                {
                    // Exclui o arquivo .bat temporário
                    if (System.IO.File.Exists(batFilePath))
                    {
                        System.IO.File.Delete(batFilePath);
                    }

                    outputFileNameTextBox.Clear();

                    if (excluifbk.IsChecked == true)
                    {
                        System.IO.File.Delete(arquivoFBK);

                        System.Windows.MessageBox.Show("Arquivo FBK excluído!");
                    }
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            // Abrir o OpenFileDialog para selecionar o arquivo FBK
            WinForms.OpenFileDialog openFileDialog = new WinForms.OpenFileDialog
            {
                Filter = "Arquivos FDB (*.fdb)|*.fdb"
            };
            if (openFileDialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                arquivoFDB = openFileDialog.FileName;

                arquivoFDBsolo = openFileDialog.SafeFileName;

                fdb.Text = arquivoFDB;

                // Open FolderBrowserDialog to select destination folder
               
            }
        }


        private void Button2_1_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == WinForms.DialogResult.OK)
                {
                    string pastaDestino = folderBrowserDialog.SelectedPath;

                    if (string.IsNullOrEmpty(textBoxContent2))
                    {
                        System.Windows.MessageBox.Show("Por favor, insira um nome para o arquivo de saída.");
                        return;
                    }


                    outputFileName2 = outputFileNameTextBox1.Text.Trim();

                    // Create the full output path
                    string outputPath = System.IO.Path.Combine(pastaDestino, outputFileName2 + ".fbk");

                    destinofbk.Text = outputPath;

                    string comando = $"cd /d \"{caminhoFire}\" && gbak -b -v -user sysdba -pass masterkey \"{arquivoFDB}\" \"{outputPath}\"";

                    // Caminho temporário para o script .bat
                    batFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "RunAsAdmin.bat");

                    // Escreve o comando no arquivo .bat, incluindo "pause" para manter o console aberto em caso de erro
                    System.IO.File.WriteAllText(batFilePath, comando + " || pause");

                    
                }
            }
        }

        private void Button2_2_Click(object sender, RoutedEventArgs e)
        {
            if (itemSelecionado == null)
            {
                System.Windows.MessageBox.Show("Por favor, selecione a pasta do GBAK do Firebird.");
                return;
            }

            outputFileNameTextBox1.Focus();
            Keyboard.ClearFocus();

            // Agora o valor deve estar atualizado
            textBoxContent2 = outputFileNameTextBox1.Text;

            if (string.IsNullOrEmpty(textBoxContent2))
            {
                System.Windows.MessageBox.Show("Por favor, insira um nome para o arquivo de saída.");
                return;
            }


            if (string.IsNullOrEmpty(arquivoFDB))
            {
                System.Windows.MessageBox.Show("Não te faz de louco, coloca o arquivo FDB.");
                return;
            }

            if (string.IsNullOrEmpty(pastaDestino))
            {
                System.Windows.MessageBox.Show("Não te faz de louco, coloca a pasta de destino.");
                return;
            }


            using (Process process = new Process())
            {
                process.StartInfo.FileName = batFilePath;
                process.StartInfo.Verb = "runas";  // Executar como administrador
                process.StartInfo.UseShellExecute = true; // Necessário para usar o Verb

                try
                {
                    process.Start();
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        System.Windows.MessageBox.Show("Conversão realizada com sucesso!");

                        gerarlogfbk(arquivoFDBsolo, pastaDestino, outputFileName2);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Ocorreu um erro durante a conversão.");
                    }
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    System.Windows.MessageBox.Show("A operação foi cancelada ou ocorreu um erro ao tentar obter privilégios administrativos.");
                }
                finally
                {
                    // Exclui o arquivo .bat temporário
                    if (System.IO.File.Exists(batFilePath))
                    {
                        System.IO.File.Delete(batFilePath);
                    }
                }
            }
        }

        private void OutputFileNameTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                // Simula o clique no botão correspondente
                Button1.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
            }
        }

        private void OutputFileNameTextBox1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                // Simula o clique no botão correspondente
                Button2.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        static void gerarlogfbk(string arquivoFDBsolo, string pastaDestino, string outputFileName2)
        {
            string original = arquivoFDBsolo;
              

            if (!Directory.Exists(@"C:\logsgbak"))
            {
                Directory.CreateDirectory(@"C:\logsgbak");
            }

            string caminho = @"C:\logsgbak";

            string novo = outputFileName2 + ".fbk";

            string nometxt = Path.Combine(caminho, $"log_fbk_de_{original}_para_{novo}.txt");

            using (StreamWriter writer = new StreamWriter(nometxt))
            {
                writer.WriteLine($"FBK {original} convertido para {novo} com sucesso!");
                writer.Write($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            }
        }

        static void gerarlogfdb(string arquivoFBKsolo, string pastaDestino, string outputFileName2)
        {
            string original = arquivoFBKsolo;


            if (!Directory.Exists(@"C:\logsgbak"))
            {
                Directory.CreateDirectory(@"C:\logsgbak");
            }

            string caminho = @"C:\logsgbak";

            string novo = outputFileName2 + ".fdb";

            string nometxt = Path.Combine(caminho, $"log_fdb_de_{original}_para_{novo}.txt");

            using (StreamWriter writer = new StreamWriter(nometxt))
            {
                writer.WriteLine($"FDB {original} convertido para {novo} com sucesso!");
                writer.Write($"{DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            }
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            outputFileNameTextBox.Clear();
            outputFileNameTextBox1.Clear();
            fdb.Text = string.Empty;
            fbk.Text = string.Empty;
            destinofbk.Text = string.Empty;
            destinofdb.Text = string.Empty;
            excluifbk.IsChecked = false;

        }
    }

}



