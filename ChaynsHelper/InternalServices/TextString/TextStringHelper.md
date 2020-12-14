## TextStringHelper

### Usage
Single Library
```c#
// Startup.cs

services.AddTextStringHelper("MyLibName", "txt_chayns_myLibPrefix_");

// File
private readonly ITextStringHelper _textString;
public MyClass(ITextStringHelper textStringHelper) 
{
    _textString = textStringHelper;
}

public void MyMethod(string name) 
{
    var myString = _textString.GetTextString(
        "my_string_name",
        "This is my textString for ##NAME##",
        new Dictionary<string, string> 
        {
            {"##NAME##", name}
        }
    );
}
```
Multiple Libraries/Languages
```c#
// Startup.cs
services.AddTextStringHelper(new Dictionary<string, TextLibOptions>
    {  
        {
            "myLibDE", 
            new TextLibOptions
            {
                Language = "Ger",
                Prefix = "txt_chayns_myLibPrefix_",
                LibName = "MyLibName"
            }
        },
        {
            "myLibEN", 
            new TextLibOptions
            {
                Language = "Eng",
                Prefix = "txt_chayns_myLibPrefix_",
                LibName = "MyLibName"
            }
        }
    }
);

// File
public void MyMethod(string name) 
{
    var myString = _textString.GetTextString(
        "my_string_name",
        "This is my textString for ##NAME##",
        new Dictionary<string, string> 
        {
            {"##NAME##", name}
        },
        libName: "myLibDE"
    );
}
```