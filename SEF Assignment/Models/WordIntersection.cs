using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using trycross.Model;

namespace trycross.Model
{
    public sealed class WordIntersection
    {
        public readonly Word Word;
        public readonly short WordCharIndex;
        public readonly Word Crossword;
        public readonly short CrosswordCharIndex;

        public WordIntersection(short wordCharIndex, short crosswordCharIndex, Word word, Word crossword)
        {
            WordCharIndex = wordCharIndex;
            CrosswordCharIndex = crosswordCharIndex;
            this.Word = word;
            this.Crossword = crossword;
        }

        public override string ToString()
        {
            return String.Format("WordCharIndex: {0} | CrosswordCharIndex: {1}", WordCharIndex, CrosswordCharIndex);
        }
    }
}
