mergeInto(LibraryManager.library,
    { 
        GetCurrentLocation: function (goName, methodName)
        {
            const checkWindow = setInterval(() => 
            {
                if (typeof window !== "undefined") 
                {
                    clearInterval(checkWindow);
                    window.unityInstance.SendMessage("Backend Bridge Service", "CatchLocation", window.location.href);
                }
            }, 100);
        },
    }
);