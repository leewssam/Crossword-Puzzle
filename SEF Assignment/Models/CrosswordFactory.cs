using System;
using System.Collections.Generic;
using System.Text;

namespace trycross.Model
{
    public sealed class CrosswordFactory
    {
        private Canvas Canvas;

        public CrosswordFactory(string[] input, ushort canvasWidth, ushort canvasHeight, bool shuffleWords)
        {
            IList<Word> words = new List<Word>();
            Canvas canvas = new Canvas(words, canvasWidth, canvasHeight, shuffleWords);

            foreach (string w in input)
            {
                if (!(w.Length > 2))
                    continue;
                Word word = new Word(w, canvas);
                words.Add(word);
            }

            this.Canvas = canvas;
        }

        public Canvas createCanvas()
        {
            return this.Canvas;
        }
    }
}
