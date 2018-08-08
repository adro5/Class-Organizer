using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;

namespace College_Organizer
{
    public class Course
    {
        public string courseName { get; set; }

        public string noteName { get; set; }

        public string notes { get; set; }

        // For future use
        public static bool operator== (Course courseA, Course courseB)
        {
            if ((courseA.courseName == courseB.courseName) && (courseA.noteName == courseB.noteName) && (courseA.notes == courseB.notes))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator!= (Course courseA, Course courseB)
        {
            if ((courseA.courseName != courseB.courseName) && (courseA.noteName != courseB.noteName) && (courseA.notes != courseB.notes))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public void Remove()
        {
            courseName = null;
            noteName = null;
            notes = null;
        }
    }
}
