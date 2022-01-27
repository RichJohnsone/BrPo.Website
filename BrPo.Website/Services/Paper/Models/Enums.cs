namespace BrPo.Website.Services.Paper.Models
{
    public class Enums
    {
        /*
         *
         * These enum values are persisted in the database - we shouldn't need to add to them.
         *
         * If we do need to add - ONLY EVER ADD TO THE END OF THE LISTS - NEVER REORDER THEM
         *
         * NEVER DELETE AN ITEM FROM THE LISTS
         *
         */

        public enum PaperSurface
        {
            Any = 0,
            Gloss = 1,
            Matt = 2,
            MetallicGloss = 3,
            Velvet = 4
        }

        public enum PaperTexture
        {
            Any = 0,
            Smooth = 1,
            Pearl = 2,
            Silk = 3,
            Lustre = 4,
            Textured = 5
        }
    }
}