# SLAB SUPERFICIES TOOL
Tool to help 西山 team of エマール group calculate superficies of multi スラブ.

## MASK
<p align='center'>
<img src='pic/0.png'></img>
</p>

## CODE DEMO
```vb
''' <summary>
''' Check internet connection.
''' </summary>
''' <returns>Connection state.</returns>
Private Function IsNetAvail()
    Dim objResp As WebResponse
    Try
        objResp = WebRequest.Create(New Uri(My.Resources.link_base)).GetResponse
        objResp.Close()
        objResp = Nothing
        Return True
    Catch ex As Exception
        Return False
    End Try
End Function
```