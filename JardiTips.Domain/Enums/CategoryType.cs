namespace JardiTips.Domain.Enums;

public enum CategoryType
{
    System = 0, // System categories are predefined and cannot be modified by users.
    User = 1,   // User categories are created and managed by users.
    Liked = 2   // Liked categories represent categories that contain Tips that were marked by the user as favorites.
}

