using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace WPFForm
{
    class Program_DES : MainWindow
    {
        private string Key { get; set; }
        private string[] Blocks;
        private string Path = "out.txt";

        private const int SizeOfBlock = 128;
        private const int ShiftKey = 2;
        private const int QuantityOfRound = 16;
        private const int SizeOfChar = 16;

        public Program_DES(string Key)
        {
            this.Key = Key;
        }

        private void OpenFile()
        {
            var file = new Process();
            file.StartInfo = new ProcessStartInfo(Path)
            {
                UseShellExecute = true
            };
            file.Start();
        }

        //Metod encription
        public void MetodEnc(string s)
        {
            string key = Key;
            string text = s;

            text = CorrectRightLength(text);
            CutStringIntoBlocks(text);

            key = CorrectKey(key, text.Length / Blocks.Length);
            key = StringToBinaryFormat(key);

            for (int i = 0; i < QuantityOfRound; i++)
            {
                for (int j = 0; j < Blocks.Length; j++)
                    Blocks[j] = EncryprionOneRound(Blocks[j], key);
                key = KeyToNextRound(key);
            }
            key = KeyToPrevRound(key);
            string result = null;
            for (int i = 0; i < Blocks.Length; i++)
                result += Blocks[i];
            using (StreamWriter sw = new StreamWriter(new FileStream(Path, FileMode.Create, FileAccess.Write), Encoding.UTF8))
            {
                sw.WriteLine("Metod Encryption");
                sw.WriteLine("\nKey : " + BinaryFormatToNormalFormat(key));
                sw.WriteLine("\nText : "+BinaryFormatToNormalFormat(result));
            }
            OpenFile();
        }

        //Metod decription
        public void MetodDec(string s)
        {
            string key = Key;
            string text = s;

            text = StringToBinaryFormat(text);
            CutBinaryStringIntoBlocks(text);
            key = StringToBinaryFormat(Key);
            for (int i = 0; i < QuantityOfRound; i++)
            {
                for (int j = 0; j < Blocks.Length; j++)
                    Blocks[j] = DecipheringOneRound(Blocks[j], key);
                key = KeyToPrevRound(key);
            }
            key = KeyToNextRound(key);
            EncKey.Text = BinaryFormatToNormalFormat(key);

            string result = null;

            for (int i = 0; i < Blocks.Length; i++)
                result += Blocks[i];
            using (StreamWriter sw = new StreamWriter(new FileStream(Path, FileMode.Create, FileAccess.Write), Encoding.UTF8))
            {
                sw.WriteLine("Metod Decryption");
                sw.WriteLine("\nKey : " + BinaryFormatToNormalFormat(key));
                sw.WriteLine("\nText : " + result_processing(BinaryFormatToNormalFormat(result)));
            }
            OpenFile();
        }

        private string result_processing(string inp)
        {
            char[] text = inp.ToCharArray();
            string result = null;
            for (int i = 0; i < text.Length; i++)
                if (text[i] != '#')
                    result += text[i].ToString();
            return result;
        }

        //Convert string normal format to binary format.
        private string StringToBinaryFormat(string input)
        {
            string output = "";
            for (int i = 0; i < input.Length; i++)
            {
                string CharBinary = Convert.ToString(input[i], 2);
                while (CharBinary.Length < SizeOfChar)
                    CharBinary = "0" + CharBinary;

                output += CharBinary;
            }
            return output;
        }

        //Convert string binary format to normal format.
        private string BinaryFormatToNormalFormat(string input)
        {
            string output = null;
            while (input.Length > 0)
            {
                string CharBinary = input.Substring(0, SizeOfChar);
                input = input.Remove(0, SizeOfChar);

                int a = 0;
                int degree = CharBinary.Length - 1;

                foreach (char CB in CharBinary)
                    a += Convert.ToInt32(CB.ToString()) * (int)Math.Pow(2, degree--);
                output += ((char)a).ToString();
            }
            return output;
        }

        //Key to next round.
        private string KeyToNextRound(string key)
        {
            for (int i = 0; i < ShiftKey; i++)
            {
                key = key[key.Length - 1] + key;
                key = key.Remove(key.Length - 1);
            }
            return key;
        }

        //Key to prev round.
        private string KeyToPrevRound(string key)
        {
            for (int i = 0; i < ShiftKey; i++)
            {
                key = key + key[0];
                key = key.Remove(0, 1);
            }
            return key;
        }

        private void CutStringIntoBlocks(string input)
        {
            Blocks = new string[(input.Length * SizeOfChar) / SizeOfBlock];
            int LenghtOfBlock = input.Length / Blocks.Length;
            for (int i = 0; i < Blocks.Length; i++)
            {
                Blocks[i] = input.Substring(i * LenghtOfBlock, LenghtOfBlock);
                Blocks[i] = StringToBinaryFormat(Blocks[i]);
            }
        }

        private void CutBinaryStringIntoBlocks(string input)
        {
            Blocks = new string[input.Length / SizeOfBlock];
            int lengthOfBlock = input.Length / Blocks.Length;
            for (int i = 0; i < Blocks.Length; i++)
                Blocks[i] = input.Substring(i * lengthOfBlock, lengthOfBlock);
        }

        private string XOR(string S1, string S2)
        {
            string result = null;
            for (int i = 0; i < S1.Length; i++)
            {
                bool text = Convert.ToBoolean(Convert.ToInt32(S1[i].ToString()));
                bool _text = Convert.ToBoolean(Convert.ToInt32(S2[i].ToString()));

                if (text ^ _text) result += "1";
                else result += "0";
            }
            return result;
        }

        private string f(string S1, string S2) { return XOR(S1, S2); }

        //Encryprion DES one round.
        private string EncryprionOneRound(string text, string key)
        {
            string L = text.Substring(0, text.Length / 2);
            string R = text.Substring(text.Length / 2, text.Length / 2);

            return R + XOR(L, f(R, key));
        }

        //Desiphering DES one round.
        private string DecipheringOneRound(string text, string key)
        {
            string L = text.Substring(0, text.Length / 2);
            string R = text.Substring(text.Length / 2, text.Length / 2);

            return (XOR(f(L, key), R) + L);
        }

        //Correct key to right length.
        private string CorrectKey(string input, int lengthKey)
        {
            if (input.Length > lengthKey)
                input = input.Substring(0, lengthKey);
            else
                while (input.Length < lengthKey)
                    input = "0" + input;
            return input;
        }

        //Correct string to right length , to be divisible by "SizeOfBlock".
        private string CorrectRightLength(string input)
        {
            while (((input.Length * SizeOfChar) % SizeOfBlock) != 0)
                input += "#";
            return input;
        }
    }
}
