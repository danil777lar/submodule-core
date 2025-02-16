mergeInto(LibraryManager.library,
    { 
        GetCurrentLocation: function (goName, methodName)
        {
            const goStr = UTF8ToString(goName);
            const meStr = UTF8ToString(methodName);
            const checkWindow = setInterval(() => 
            {
                if ((typeof window !== "undefined") && (typeof window.unityInstance !== "undefined") && (typeof window.unityInstance.SendMessage === "function"))  
                {
                    window.unityInstance.SendMessage(goStr, meStr, window.location.href);
                    clearInterval(checkWindow);
                }
            }, 1000);
        },
    }
);