using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace ShingleAlgorithm
{
    public partial class MainWindow : Window
    {
        OpenFileDialog OFD = new OpenFileDialog();
        public MainWindow()
        {
            InitializeComponent();    
        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            if (OFD.ShowDialog() == true)
            {
                Tb1.Text = OFD.FileName;
            }
        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            if (OFD.ShowDialog() == true)
            {
                Tb2.Text = OFD.FileName;
            }
        }

        private void BtnCompare_Click(object sender, RoutedEventArgs e)
        {
            //text file path
            string _pathText1 = Tb1.Text;
            string _pathText2 = Tb2.Text;

            //file reading
            string _txt1 = File.ReadAllText(_pathText1, Encoding.Default);
            string _txt2 = File.ReadAllText(_pathText2, Encoding.Default);

            //text cononization
            string _normalizationTxt1 = NarmalizeSentence(_txt1);
            string _normalizationTxt2 = NarmalizeSentence(_txt2);

            //get shingles
            var _shinglesTxt1 = GetShingles(_normalizationTxt1, 10);
            var _shinglesTxt2 = GetShingles(_normalizationTxt2, 10);


            //get the hash table
            var _txt1Hash = GetHashTable(_shinglesTxt1);
            var _txt2Hash = GetHashTable(_shinglesTxt2);

            //compare
            var _result = CompareShingles(_txt1Hash, _txt2Hash);
            tbResult.Text = _result + "%";
        }

        //text cononization method
        private bool IsNormalChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == ' ';
        }

        private string NarmalizeSentence(string sentence)
        {
            var result = new StringBuilder();
            var lowerSentence = sentence.ToLower();
            foreach (var c in lowerSentence)
            {
                if (IsNormalChar(c))
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        //break sentences into words
        private string[] GetWords(string sentence)
        {
            var tokens = new List<string>();
            var words = sentence.Split(' ');
            foreach (var w in words)
            {
                if (w.Length >= 2)
                {
                    tokens.Add(w);
                }
            }
            return tokens.ToArray();
        }

        //getting shingles
        private IEnumerable<string> GetShingles(string sentence, int shingleLength)
        {
            var words = GetWords(sentence);
            for (int i = 0; i < words.Length - shingleLength + 1; i++)
                yield return string.Join(" ", words, i, shingleLength);
        }

        //hash
        private int[] GetHashTable(IEnumerable<string> shingles)
        {
            var hash = new List<int>();
            foreach (var w in shingles)
            {
                hash.Add(w.GetHashCode());
            }
            return hash.ToArray();
        }

        //hash table comparison
        private float CompareShingles(int[] arr1, int[] arr2)
        {
            float _count = 0;

            for (int i = 0; i < arr1.Length; i++)
            {
                for (int j = 0; j < arr2.Length; j++)
                {
                    if (arr1[i] == arr2[j])
                        _count++;
                }
            }
            return _count * 2 / (arr1.Length + arr2.Length) * 100f;
        }

    }
}
