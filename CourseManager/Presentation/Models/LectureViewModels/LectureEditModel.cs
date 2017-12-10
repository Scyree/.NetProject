﻿using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Presentation.Models
{
    public class LectureEditModel
    {
        public LectureEditModel()
        {
        }

        [Required(ErrorMessage = "A title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "A description is required.")]
        [StringLength(2000, ErrorMessage = "Maximum number of characters is 2000!")]
        public string Description { get; set; }
        
        [Required]
        public IEnumerable<IFormFile> File { get; set; }

        public List<string> GetFiles()
        {
            List<string> fileList = new List<string>();
            string path = Directory.GetCurrentDirectory() + "\\wwwroot\\Lectures\\" + Title;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (var files in Directory.GetFiles(path))
            {
                fileList.Add(Path.GetFileName(files));
            }

            return fileList;
        }

        public LectureEditModel(string title, string description)
        {
            Title = title;
            Description = description;
        }

    }
}