﻿using PropertyChanged;

namespace Models
{
    [AddINotifyPropertyChangedInterface]
    [Serializable]
    public class Cell
    {
        public int Id { get; set; }
        public bool IsContainsShip { get; set; }
        public bool IsHit { get; set; }

        public Field Field { get; set; } = null!;
    }
}
