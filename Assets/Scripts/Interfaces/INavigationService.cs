// Interface for navigation - makes scene loading testable
public interface INavigationService
{
    void GoTo(string sceneName);
    void GoBack();
    string CurrentScene { get; }
}
