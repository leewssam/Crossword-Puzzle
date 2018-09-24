using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using trycross.Model;

namespace trycross.Model
{
    public sealed class Word
    {
        public string word;
        public short X;
        public short Y;
        public bool[] AvailableChar { get; private set; }
        public IList<WordIntersection> Crosswords;
        public Orientation Orientation;
        public Canvas Canvas;
        public int Length
        {
            get
            {
                return this.word.Length;
            }
        }

        public Word(string word, Canvas canvas)
        {
            this.word = word.ToUpper();

            Random rnd = new Random();
            this.AvailableChar = new bool[word.Length];

            for (ushort i = 0; i < this.AvailableChar.Length; i++)
            {
                this.AvailableChar[i] = true;
            }

            //Array.Sort(this.AvailableChar);

            this.Crosswords = new List<WordIntersection>();
            this.Orientation = (Orientation)rnd.Next(2);
            //this.Orientation = Orientation.Horizontal;

            if (this.Orientation == Orientation.Vertical)
            {
                this.X = (short)rnd.Next(Math.Max(0, (int)canvas.Width));
                this.Y = (short)rnd.Next(Math.Max(0, (int)canvas.Height - this.Length));
            }
            else if (this.Orientation == Orientation.Horizontal)
            {
                this.X = (short)rnd.Next(Math.Max(0, (int)canvas.Width - this.Length));
                this.Y = (short)rnd.Next(Math.Max(0, (int)canvas.Height));
            }

            this.Canvas = canvas;
        }

        public WordIntersection MatchedChar(Word _word)
        {
            char[] thisChars = this.word.ToCharArray();
            char[] wordChars = _word.word.ToCharArray();
            WordIntersection returnValue = default(WordIntersection);
            short? smallestCrosswordWordIntersectIndex = null;
            short? smallestWordIntersectIndex = null;

            for (short i = 0; i < wordChars.Length; i++)
            {
                for (short n = 0; n < thisChars.Length; n++)
                {
                    if (this.AvailableChar[n] && _word.AvailableChar[i] && thisChars[n].Equals(wordChars[i]))
                    {
                        if (smallestCrosswordWordIntersectIndex == null || i < smallestCrosswordWordIntersectIndex)
                        {
                            smallestCrosswordWordIntersectIndex = i;
                            smallestWordIntersectIndex = n;
                        }
                    }
                }
            }

            if (smallestWordIntersectIndex != null && smallestCrosswordWordIntersectIndex != null)
            {
                short n = (short)smallestWordIntersectIndex;
                short i = (short)smallestCrosswordWordIntersectIndex;

                returnValue = new WordIntersection(n, i, this, _word);
            }

            return returnValue;
        }

        public override string ToString()
        {
            return this.word;
        }

        public void AddCrossword(Word word)
        {
            if (this == word)
            {
                return;
            }

            WordIntersection matchIntersection = this.MatchedChar(word);

            if (matchIntersection == null)
            {
                return;
            }
            else
            {
                short CharIndex = matchIntersection.WordCharIndex;
                short CrosswordCharIndex = matchIntersection.CrosswordCharIndex;

                if (this.Orientation == Orientation.Vertical)
                {
                    word.X = (short)(this.X - CrosswordCharIndex);
                    word.Y = (short)(this.Y + CharIndex);
                    word.Orientation = Orientation.Horizontal;
                }
                else if (this.Orientation == Orientation.Horizontal)
                {
                    word.X = (short)(this.X + CharIndex);
                    word.Y = (short)(this.Y - CrosswordCharIndex);
                    word.Orientation = Orientation.Vertical;
                }

                if (word.X >= 0 && word.Y >= 0 && this.Canvas.CanAdd(word))
                {

                    this.AvailableChar[CharIndex] = false;
                    word.AvailableChar[CrosswordCharIndex] = false;

                    if (CharIndex - 1 >= 0) this.AvailableChar[CharIndex - 1] = false;
                    if (CrosswordCharIndex - 1 >= 0) word.AvailableChar[CrosswordCharIndex - 1] = false;
                    if (CharIndex + 1 < this.AvailableChar.Length) this.AvailableChar[CharIndex + 1] = false;
                    if (CrosswordCharIndex + 1 < word.AvailableChar.Length) word.AvailableChar[CrosswordCharIndex + 1] = false;

                    this.Crosswords.Add(matchIntersection);
                    word.Crosswords.Add(new WordIntersection(CrosswordCharIndex, CharIndex, word, this));
                    this.Canvas.AddWord(word);
                }
            }
        }
    }
}
