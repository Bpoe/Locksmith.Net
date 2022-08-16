namespace Locksmith.Core
{
    public static class Errors
    {
        public static readonly Error ErrExist = new Error("holder exists");

        public static readonly Error ErrNotExist = new Error("holder does not exist");
    }
}
