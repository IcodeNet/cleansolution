Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Microsoft.ApplicationServer.Http
Imports System.Net.Http


<ServiceContract()>
Public Class MyService
    <WebInvoke(UriTemplate:="/", Method:="POST")>
    Public Sub Post(request As HttpRequestMessage, value As Int32)
        Dim val = request.Content.ReadAs(Of Int32)()
    End Sub
End Class
