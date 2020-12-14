## [RequestInfoContextProvider](./RequestInfoContextProvider.cs)
Provider to set Logger defaults:
* ``location_id`` and ``person_id`` from Token if present
* Route (Method & Path) in ``custom_text`` 
 
  
The NuGet-Package ``TobitWebApiExtensionsCore`` must be installed.

#### LoggerProvider must be changed in ``Startup.cs``
```c#
services.UseRequestInfoContext();
```
This will replace any previous registration of `ILogContextProvider`.