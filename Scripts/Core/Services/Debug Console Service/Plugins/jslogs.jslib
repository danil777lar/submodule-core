mergeInto(LibraryManager.library, 
{
    RegisterLogCallback: function(className, methodName) 
    {
        console.log = function(message) 
        {
            if (window.unityInstance)
            {
                window.unityInstance.SendMessage("Debug Console Service", "HandleJSLog", message);
            }
        };
    }
});