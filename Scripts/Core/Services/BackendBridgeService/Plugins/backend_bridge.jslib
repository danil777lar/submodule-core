mergeInto(LibraryManager.library,
{ 
    GetCurrentLocation: function (goName, methodName)
    {
        goNameStr = UTF8ToString(goName);
        methodNameStr = UTF8ToString(methodName);
        window.unityInstance.SendMessage(goNameStr, methodNameStr, path.ToString());
    },
};