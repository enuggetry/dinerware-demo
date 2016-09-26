Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Threading

Module Module1

    Sub Main()

        Dim dinerware As New DinerWare1.VirtualClientClient
        Dim cki As ConsoleKeyInfo


        Do

            Dim tickets As List(Of DinerWare1.wsTicket)
            tickets = dinerware.getOpenTickets().ToList

            Dim payments As New List(Of DinerWare1.Payment)
            Dim items As New List(Of DinerWare1.wsMenuItem)

            Dim json As String
            json = "{""tickets"":["
            Dim i As Integer = 0
            Dim j As Integer = 0

            For Each t As DinerWare1.wsTicket In tickets
                Console.WriteLine("Customer: " + t.Name + " ticket: " + t.ID)

                If i > 0 Then json = json & ","
                json = json & "{""customer"":""" & t.Name & """,""tid"":""" & t.ID & """,""amount_due"":""" & CType(t.AmountDue, String) & """"

                items = dinerware.GetTicketMenuItems(0, t.ID).ToList
                j = 0
                If items.Count > 0 Then json = json & ",""items"":["
                For Each item1 As DinerWare1.wsMenuItem In items
                    If j > 0 Then json = json & ","
                    Console.WriteLine("Item: " + item1.NAME + " " + CType(item1.NET_PRICE, String))
                    json = json & "{""item"":""" & item1.NAME & """,""price"":""" & item1.NET_PRICE & """}"
                    j = j + 1
                Next
                If items.Count > 0 Then json = json & "]"

                payments.Clear()
                payments.AddRange(dinerware.getTransForTicket(t.ID, False))
                j = 0
                If payments.Count > 0 Then json = json & ",""payments"":["
                For Each payment As DinerWare1.Payment In payments
                    If j > 0 Then json = json & ","
                    Console.WriteLine("Payment: " + CType(payment.TenderedAmount, String))
                    json = json & "{""payment"":""" & payment.TenderedAmount & """,""time"":""" & payment.Time & """}"
                    j = j + 1
                Next
                If payments.Count > 0 Then json = json & "]"

                Console.WriteLine("Amount Due: " + CType(t.AmountDue, String))
                json = json & "}"

                i = i + 1
            Next
            json = json & "]}"

            Dim uri As Uri = New Uri("http://drinkcode.rooby.me/dinerwaretest.php")

            Dim data = Encoding.UTF8.GetBytes(json)
            Dim result_post = SendRequest(uri, data, "application/json", "POST")

            Console.WriteLine("JSON Return: " + result_post)

            'GoTo end_of_line

            Thread.Sleep(2000)

        Loop While 1

end_of_line:

        Dim input = Console.ReadLine()
    End Sub

    Private Function SendRequest(uri As Uri, jsonDataBytes As Byte(), contentType As String, method As String) As String
        Dim req As WebRequest = WebRequest.Create(uri)
        req.ContentType = contentType
        req.Method = method
        req.ContentLength = jsonDataBytes.Length


        Dim stream = req.GetRequestStream()
        stream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
        stream.Close()

        Dim response = req.GetResponse().GetResponseStream()

        Dim reader As New StreamReader(response)
        Dim res = reader.ReadToEnd()
        reader.Close()
        response.Close()

        Return res
    End Function
End Module
