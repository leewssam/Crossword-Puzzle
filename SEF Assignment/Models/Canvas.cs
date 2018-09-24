using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web.UI.WebControls;
using trycross.Model;

namespace trycross.Model
{
    public sealed class Canvas
    {
        public IList<Word> WordsAtCanvas;
        public IList<Word> Words;
        public char[][] Matrix;
        public readonly ushort Width;
        public readonly ushort Height;
        public readonly bool Shuffle;
        public IList<CanvasCharacter> Chars;

        public Canvas(IList<Word> words, ushort canvasWidth, ushort canvasHeight, bool shuffleWords)
        {
            Words = words;
            WordsAtCanvas = new List<Word>();
            this.Width = canvasWidth;
            this.Height = canvasHeight;
            this.Shuffle = shuffleWords;
            this.Chars = new List<CanvasCharacter>();

            Matrix = new char[this.Width][];
            for (byte i = 0; i < this.Width; i++)
            {
                Matrix[i] = new char[this.Height];
            }

        }

        public void Build()
        {
            
            if (!(this.Words.Count > 0))
            {
                throw new ArgumentException();
            }

            Word firstWord = default(Word);

            for (ushort i = 1; i < this.Words.Count; i++)
            {
                if (this.Words[i].Length > Height || this.Words[i].Length > Height)
                {
                    this.Words.Remove(this.Words[i]);
                    continue;
                }
                if (firstWord == null || this.Words[i].Length > firstWord.Length)
                {
                    firstWord = this.Words[i];
                }
            }
            AddWord(firstWord);

            if (this.Shuffle)
                ShuffleWords();

            int fillTimes = 0, maxFillTimes = this.Words.Count;
            while (this.Words.Count > 0 && fillTimes++ < maxFillTimes) FillCanvas();

        }

        private void FillCanvas()
        {
            for (ushort i = 0; i < this.WordsAtCanvas.Count; i++)
            {
                for (ushort n = 0; n < this.Words.Count; n++)
                {
                    this.WordsAtCanvas[i].AddCrossword(this.Words[n]);
                }
            }

            for (ushort i = 0; i < this.WordsAtCanvas.Count; i++)
            {
                for (ushort n = 1; n <= this.Words.Count; n++)
                {
                    this.WordsAtCanvas[i].AddCrossword(this.Words[this.Words.Count - n]);
                }
            }

            for (ushort i = 0; i < this.Words.Count; i++)
            {
                for (ushort n = 0; n < this.WordsAtCanvas.Count; n++)
                {
                    this.WordsAtCanvas[n].AddCrossword(this.Words[i]);
                }
            }

            for (ushort i = 0; i < this.Words.Count; i++)
            {
                for (ushort n = 1; n <= this.WordsAtCanvas.Count; n++)
                {
                    if (i < this.Words.Count) this.WordsAtCanvas[WordsAtCanvas.Count - n].AddCrossword(this.Words[i]);
                }
            }
        }

        public bool CanAdd(Word word)
        {
            bool canAdd = true;
            if (word.Orientation == Orientation.Horizontal)
            {
                if (word.X - 1 >= 0)
                    if (this.Matrix[word.X - 1][word.Y] != Char.MinValue)
                        return false;


                if (word.X + word.Length < this.Width)
                {
                    if (this.Matrix[word.X + word.Length][word.Y] != Char.MinValue)
                        return false;
                }
                else
                {
                    return false;
                }

                for (ushort i = 0; i < word.Length; i++)
                {
                    if (this.Matrix[word.X + i][word.Y] != Char.MinValue && word.ToString()[i] != this.Matrix[word.X + i][word.Y])
                    {
                        canAdd = false;
                        break;
                    }
                }
            }
            else if (word.Orientation == Orientation.Vertical)
            {
                if (word.Y - 1 >= 0)
                    if (this.Matrix[word.X][word.Y - 1] != Char.MinValue)
                        return false;

                if (word.Y + word.Length < this.Height)
                {
                    if (this.Matrix[word.X][word.Y + word.Length] != Char.MinValue)
                        return false;
                }
                else
                {
                    return false;
                }

                for (ushort i = 0; i < word.Length; i++)
                {
                    if (word.Y + i > this.Height - 1) return false;
                    if (this.Matrix[word.X][word.Y + i] != Char.MinValue && word.ToString()[i] != this.Matrix[word.X][word.Y + i])
                    {
                        canAdd = false;
                        break;
                    }
                }
            }

            return canAdd;
        }

        public void AddWord(Word word)
        {
            if (word.Orientation == Orientation.Horizontal)
            {
                if (word.X - 1 >= 0) this.Matrix[word.X - 1][word.Y] = '\n';
                if (word.X + word.Length < this.Width) this.Matrix[word.X + word.Length][word.Y] = '\n';

                for (ushort i = 0; i < word.Length; i++)
                {
                    Chars.Add(new CanvasCharacter(word.ToString()[i], i, word, word.X + i, word.Y));
                    if (word.ToString()[i] == this.Matrix[word.X + i][word.Y])
                    {
                        DisableCharsAt(word.X + i, word.Y);
                    }
                    this.Matrix[word.X + i][word.Y] = word.ToString()[i];
                }
            }
            else if (word.Orientation == Orientation.Vertical)
            {
                if (word.Y - 1 >= 0) this.Matrix[word.X][word.Y - 1] = '\n';
                if (word.Y + word.Length < this.Height) this.Matrix[word.X][word.Y + word.Length] = '\n';

                for (ushort i = 0; i < word.Length; i++)
                {
                    Chars.Add(new CanvasCharacter(word.ToString()[i], i, word, word.X, word.Y + i));
                    if (word.ToString()[i] == this.Matrix[word.X][word.Y + i])
                    {
                        DisableCharsAt(word.X, word.Y + i);
                    }

                    this.Matrix[word.X][word.Y + i] = word.ToString()[i];
                }
            }

            WordsAtCanvas.Add(word);
            Words.Remove(word);
        }

        private void DisableCharsAt(int x, int y)
        {
            for (int i = 0; i < Chars.Count; i++)
            {
                if (Chars[i].Word.Orientation == Orientation.Vertical)
                {
                    if (Chars[i].Word.X == x && (Chars[i].Word.Y + Chars[i].Index) == y)
                    {
                        if (Chars[i].Index - 1 >= 0) Chars[i].Word.AvailableChar[Chars[i].Index - 1] = false;
                        if (Chars[i].Index + 1 < Chars[i].Word.AvailableChar.Length) Chars[i].Word.AvailableChar[Chars[i].Index + 1] = false;
                        Chars[i].Word.AvailableChar[Chars[i].Index] = false;
                    }
                }
                else if (Chars[i].Word.Orientation == Orientation.Horizontal)
                {
                    if (Chars[i].Word.Y == y && Chars[i].Word.X <= x && (Chars[i].Word.X + Chars[i].Index) == x)
                    {
                        if (Chars[i].Index - 1 >= 0) Chars[i].Word.AvailableChar[Chars[i].Index - 1] = false;
                        if (Chars[i].Index + 1 < Chars[i].Word.AvailableChar.Length) Chars[i].Word.AvailableChar[Chars[i].Index + 1] = false;
                        Chars[i].Word.AvailableChar[Chars[i].Index] = false;
                    }
                }
            }
        }

        public void ShuffleWords()
        {
            Random rng = new Random();
            int n = this.Words.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Word value = this.Words[k];
                this.Words[k] = this.Words[n];
                this.Words[n] = value;
            }
        }
    }
}
