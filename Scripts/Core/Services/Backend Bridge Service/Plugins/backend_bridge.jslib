mergeInto(LibraryManager.library,
    { 
        GetCurrentLocation: function (goName, methodName)
        {
            goStr = UTF8ToString(goName);
            meStr = UTF8ToString(methodName);

            const checkWindow = setInterval(() => 
            {
                if (typeof window !== "undefined") 
                {
                
                    console.log(goStr);
                    console.log(meStr);

                    clearInterval(checkWindow);
                    window.unityInstance.SendMessage(goStr, meStr, window.location.href);
                }
            }, 100);
        },
    }
);