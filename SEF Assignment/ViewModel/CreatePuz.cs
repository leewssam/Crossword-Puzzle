using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SEF_Assignment.ViewModel
{
    public class CreatePuz
    {
        [Required(ErrorMessage = "Puzzle Name is required!")]
        public String PuzzleName { get; set; }

        [Required(ErrorMessage = "Please enter question!")]
        public String Question { get; set; }

        [Required(ErrorMessage = "Please enter answer!")]
        public String Answer { get; set; }

        [Required(ErrorMessage = "Please enter puzzle score!")]
        public int PuzzleScore { get; set; }

        [Required(ErrorMessage = "Please enter puzzle hint!")]
        public int PuzzleHint { get; set; }
    }
}