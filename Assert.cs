namespace ImageSearchBot
{
    public static class Assert
    {

        public static void Check(bool condition, string message)
        {
            if (!condition) 
            {
                throw new System.InvalidOperationException(message);
            }
        }

    }    
}