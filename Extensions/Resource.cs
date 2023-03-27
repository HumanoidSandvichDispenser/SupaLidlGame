using Godot;

namespace SupaLidlGame.Extensions
{
    public static class ResourceExtensions
    {
        public static string GetFileName(this Resource resource)
        {
            return resource.ResourcePath.GetFile();
        }
    }
}
