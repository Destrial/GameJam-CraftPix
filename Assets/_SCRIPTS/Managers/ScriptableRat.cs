using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Destrial
{

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Rat", order = 1)]
    public class ScriptableRat : ScriptableObject
    {
        public string RatName;
        public string RatDescription;
        
      
        public Color[] Colors;
        
        public enum RatForme
        {
            Box,
            Circle,
            Triangle
        }
       
        public enum RatColor
        {
            White,
            Grey,
            Brown,
            Red,
            Blue,
            Yellow }
        

        public bool[] Lies;
        
        public Sprite[] RatSpriteTop;
        public Sprite[] RatSpriteBottom;
        
        public bool IsMutant;
        public RatForme RatFormeHead;
        public RatForme RatFormeBody;
        public Sprite RatTrueSpriteTop;
        public Sprite RatTrueSpriteBottom;

        public List<string> RandomDialog;


    }
}
