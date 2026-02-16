Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Net.Mail

Public Class MailingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Function GetDataRow(thisString As String) As DataRow
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using thisAdapter As New SqlDataAdapter(thisCmd)
                        Dim dt As New DataTable()
                        thisAdapter.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            Return dt.Rows(0)
                        Else
                            Return Nothing
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetDataTable(thisString As String) As DataTable
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using da As New SqlDataAdapter(thisCmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return dt
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetItemData(thisString As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0).ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetItemData_Boolean(thisString As String) As Boolean
        Dim result As Boolean = False
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0)
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function

    Public Sub WebError(data As Object())
        Try
            Dim loginId As String = Convert.ToString(data(0))
            Dim companyId As String = Convert.ToString(data(1))
            Dim page As String = Convert.ToString(data(2))
            Dim action As String = Convert.ToString(data(3))
            Dim description As String = Convert.ToString(data(4))

            Dim mailData As DataRow = GetDataRow("SELECT * FROM Mailings WHERE CompanyId='" & companyId & "' AND Name='Web Error' AND Active=1")

            If mailData Is Nothing Then Exit Sub

            Dim mailServer As String = mailData("Server").ToString()
            Dim mailHost As String = mailData("Host").ToString()
            Dim mailPort As String = mailData("Port").ToString()

            Dim mailAccount As String = mailData("Account").ToString()
            Dim mailPassword As String = mailData("Password").ToString()
            Dim mailAlias As String = mailData("Alias").ToString()
            Dim mailSubject As String = mailData("Subject").ToString()

            Dim mailTo As String = mailData("To").ToString()
            If String.IsNullOrEmpty(mailTo) Then mailTo = "reza@bigblinds.co.id"
            Dim mailCc As String = mailData("Cc").ToString()
            Dim mailBcc As String = mailData("Bcc").ToString()

            Dim mailNetworkCredentials As Boolean = mailData("NetworkCredentials")
            Dim mailDefaultCredentials As Boolean = mailData("DefaultCredentials")
            Dim mailEnableSSL As Boolean = mailData("EnableSSL")

            Dim userName As String = GetItemData("SELECT FullName FROM CustomerLogins WHERE Id='" & loginId & "'")

            Dim mailBody As String = String.Empty

            mailBody = "Hi Team, there's an error."
            mailBody &= "<br /><br />"
            mailBody &= "Web Page : " & page
            mailBody &= "<br />"
            mailBody &= "Action : " & action
            mailBody &= "<br />"
            mailBody &= "Users : " & loginId & " | " & userName
            mailBody &= "<br /><br />"
            mailBody &= "Error Message : "
            mailBody &= "<br />"
            mailBody &= description

            Using myMail As New MailMessage()
                myMail.Subject = mailSubject
                myMail.From = New MailAddress(mailServer, mailAlias)
                myMail.To.Add(mailTo)

                If Not mailCc = "" Then
                    Dim thisArray() As String = mailCc.Split(";")
                    Dim thisMail As String = String.Empty
                    For Each thisMail In thisArray
                        myMail.CC.Add(thisMail)
                    Next
                End If

                If Not mailBcc = "" Then
                    Dim thisArray() As String = mailBcc.Split(";")
                    Dim thisMail As String = String.Empty
                    For Each thisMail In thisArray
                        myMail.Bcc.Add(thisMail)
                    Next
                End If

                myMail.Body = mailBody
                myMail.IsBodyHtml = True
                Dim smtpClient As New SmtpClient()
                smtpClient.Host = mailHost
                smtpClient.EnableSsl = mailEnableSSL
                Dim NetworkCredl As New NetworkCredential()
                NetworkCredl.UserName = mailAccount
                NetworkCredl.Password = mailPassword
                smtpClient.UseDefaultCredentials = mailDefaultCredentials
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network

                If mailNetworkCredentials = True Then
                    smtpClient.Credentials = NetworkCredl
                End If

                smtpClient.Port = mailPort
                smtpClient.Send(myMail)
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Sub MailUnshipment(fileDirectory As String)
        Try
            Dim mailData As DataRow = GetDataRow("SELECT TOP 1 * FROM Mailings WHERE Active = 1 AND Name = 'In Production - Unshipment'")
            If mailData Is Nothing Then Exit Sub

            Dim mailServer As String = mailData("Server").ToString()
            Dim mailHost As String = mailData("Host").ToString()
            Dim mailPort As String = mailData("Port").ToString()

            Dim mailAccount As String = mailData("Account").ToString()
            Dim mailPassword As String = mailData("Password").ToString()
            Dim mailAlias As String = mailData("Alias").ToString()
            Dim mailSubject As String = mailData("Subject").ToString()

            Dim mailTo As String = mailData("To").ToString()
            Dim mailCc As String = mailData("Cc").ToString()
            Dim mailBcc As String = mailData("Bcc").ToString()

            Dim mailNetworkCredentials As Boolean = mailData("NetworkCredentials")
            Dim mailDefaultCredentials As Boolean = mailData("DefaultCredentials")
            Dim mailEnableSSL As Boolean = mailData("EnableSSL")

            Dim mailContent As String = "<span style='font-family: Cambria; font-size: 14px;'>Hi Galih & Indra,</span>"
            mailContent += "<br /><br />"
            mailContent += "Please see the attached file."
            mailContent += "<br /><br /><br />"
            mailContent += "<span style='font-family: Cambria; font-size:16px;'>Kind Regards,</span>"
            mailContent += "<br />"
            mailContent += "<span style='font-family: Cambria; font-size:16px; font-weight: bold;'>Reza Andika Pratama</span><span style='font-family: Cambria; font-size:16px;> | IT Support</span>"
            mailContent += "<br />"
            mailContent += "<span style='font-family: Cambria; font-size:16px;>reza@bigblinds.co.id</span>"
            mailContent += "<br />"

            Dim myMail As New MailMessage

            If Not mailTo = "" Then
                Dim toArray() As String = mailTo.Split(";")
                Dim thisMail As String = String.Empty
                For Each thisMail In toArray
                    myMail.To.Add(thisMail)
                Next
            Else
                myMail.To.Add("reza@bigblinds.co.id")
            End If

            If Not mailCc = "" Then
                Dim ccArray() As String = mailCc.Split(";")
                Dim thisMail As String = String.Empty
                For Each thisMail In ccArray
                    myMail.CC.Add(thisMail)
                Next
            End If

            If Not mailBcc = "" Then
                Dim thisArray() As String = mailBcc.Split(";")
                Dim thisMail As String = String.Empty
                For Each thisMail In thisArray
                    myMail.Bcc.Add(thisMail)
                Next
            End If

            myMail.Subject = mailSubject
            myMail.From = New MailAddress(mailServer, mailAlias)
            myMail.Body = mailContent
            myMail.Attachments.Add(New Attachment(fileDirectory))
            myMail.IsBodyHtml = True
            Dim smtpClient As New SmtpClient()
            smtpClient.Host = mailHost
            smtpClient.EnableSsl = mailEnableSSL

            Dim NetworkCredl As New NetworkCredential()
            NetworkCredl.UserName = mailAccount
            NetworkCredl.Password = mailPassword
            smtpClient.UseDefaultCredentials = mailDefaultCredentials
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
            If mailNetworkCredentials = True Then
                smtpClient.Credentials = NetworkCredl
            End If
            smtpClient.Port = mailPort
            smtpClient.Send(myMail)
        Catch ex As Exception
        End Try
    End Sub

    Public Sub NewOrder(headerId As String, loginId As String)
        Try
            If String.IsNullOrEmpty(headerId) Then Exit Sub

            Dim previewClass As New PreviewClass
            Dim pdfBytes As Byte() = previewClass.BindContent(headerId)

            Dim orderData As DataRow = GetDataRow("SELECT OrderHeaders.*, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id='" & headerId & "'")
            If orderData Is Nothing Then Exit Sub

            Dim customerId As String = orderData("CustomerId").ToString()
            Dim orderId As String = orderData("OrderId").ToString()
            Dim orderNumber As String = orderData("OrderNumber").ToString()
            Dim orderName As String = orderData("OrderName").ToString()
            Dim orderNote As String = orderData("OrderNote").ToString()

            Dim safeNote As String = HttpUtility.HtmlEncode(orderNote)
            safeNote = safeNote.Replace(vbCrLf, "<br>").Replace(vbLf, "<br>")

            Dim customerName As String = orderData("CustomerName").ToString()
            Dim companyId As String = orderData("CompanyId").ToString()
            Dim companyName As String = GetItemData("SELECT Name FROM Companys WHERE Id='" & companyId & "'")

            Dim submitRole As String = GetItemData("SELECT LoginRoles.Name AS RoleName FROM CustomerLogins LEFT JOIN LoginRoles ON CustomerLogin.RoleId=LoginRoles.Id WHERE CustomerLogins.Id='" & loginId & "'")

            Dim mailData As DataRow = GetDataRow("SELECT * FROM Mailings WHERE CompanyId='" & companyId & "' AND Name='New Order' AND Active=1")
            If mailData Is Nothing Then Exit Sub

            Dim mailServer As String = mailData("Server").ToString()
            Dim mailHost As String = mailData("Host").ToString()
            Dim mailPort As Integer = mailData("Port")

            Dim mailAccount As String = mailData("Account").ToString()
            Dim mailPassword As String = mailData("Password").ToString()
            Dim mailAlias As String = mailData("Alias").ToString()
            Dim mailSubject As String = mailData("Subject").ToString()

            Dim mailTo As String = mailData("To").ToString()
            Dim mailCc As String = mailData("Cc").ToString()
            Dim mailBcc As String = mailData("Bcc").ToString()

            Dim mailNetworkCredentials As Boolean = mailData("NetworkCredentials")
            Dim mailDefaultCredentials As Boolean = mailData("DefaultCredentials")
            Dim mailEnableSSL As Boolean = mailData("EnableSSL")

            Dim mailBody As String = String.Empty

            Dim submitBy As String = "A new order has been submitted by customer."
            If Not String.IsNullOrEmpty(submitRole) AndAlso Not submitRole = "Customer" Then
                submitBy = String.Format("A new order has been submitted by <b>{0}</b>.", submitBy.ToLower)
            End If

            mailBody = "<span style='font-family: Cambria; font-size: 16px;'>"
            mailBody &= submitBy
            mailBody &= "<br /><br />"
            mailBody &= "Please check and download it from the BOE for production processing."
            mailBody &= "</span>"

            mailBody &= "<br /><br /><br />"

            mailBody &= "<table cellpadding='3' cellspacing='0' style='font-family:Cambria; font-size: 16px;'>"
            mailBody &= "<tr><td valign='top'>Company</td><td valign='top'>:</td><td valign='top'><b>" & companyName & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Customer Name</td><td valign='top'>:</td><td valign='top'><b>" & customerName & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Order #</td><td valign='top'>:</td><td valign='top'><b>" & orderId & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Order Number</td><td valign='top'>:</td><td valign='top'><b>" & orderNumber & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Order Name</td><td valign='top'>:</td><td valign='top'><b>" & orderName & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Order Note</td><td valign='top'>:</td><td valign='top'><b>" & safeNote & "</b></td></tr>"
            mailBody &= "</table>"

            mailBody &= "<br /><br />"

            mailBody &= "<span style='font-family: Cambria; font-size:16px;'>Kind Regards,</span>"
            mailBody &= "<br /><br /><br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px; font-weight: bold;'>Reza Andika Pratama</span><span style='font-family: Cambria; font-size:16px;'> | Developer</span>"
            mailBody &= "<br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px; font-weight: bold; color: red;'>E</span><span style='font-family: Cambria; font-size:16px;'> : reza@bigblinds.co.id</span>"
            mailBody &= "<br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px; font-weight: bold; color: red;'>P</span><span style='font-family: Cambria; font-size:16px;'> : +62 852 1504 3355</span>"

            Dim myMail As New MailMessage

            Dim subject As String = String.Format("{0} - {1} - {2} - New Order # {3}", customerName, orderNumber, orderName, orderId)

            myMail.Subject = subject
            myMail.From = New MailAddress(mailServer, mailAlias)

            If Not String.IsNullOrEmpty(mailTo) Then
                Dim thisArray() As String = mailTo.Split(";")
                Dim thisMail As String = String.Empty
                For Each thisMail In thisArray
                    myMail.To.Add(thisMail)
                Next
            End If

            If Not mailCc = "" Then
                Dim thisArray() As String = mailCc.Split(";")
                Dim thisMail As String = String.Empty
                For Each thisMail In thisArray
                    myMail.CC.Add(thisMail)
                Next
            End If

            If Not mailBcc = "" Then
                Dim thisArray() As String = mailBcc.Split(";")
                Dim thisMail As String = String.Empty
                For Each thisMail In thisArray
                    myMail.Bcc.Add(thisMail)
                Next
            End If

            myMail.Body = mailBody
            myMail.IsBodyHtml = True

            Dim ms As New MemoryStream(pdfBytes)
            Dim attach As New Attachment(ms, "Order_" & orderId & ".pdf", "application/pdf")
            myMail.Attachments.Add(attach)

            Dim smtpClient As New SmtpClient()
            smtpClient.Host = mailHost
            smtpClient.Port = mailPort
            smtpClient.EnableSsl = mailEnableSSL
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
            smtpClient.Timeout = 120000

            If mailNetworkCredentials Then
                smtpClient.UseDefaultCredentials = False
                smtpClient.Credentials = New NetworkCredential(mailAccount, mailPassword)
            Else
                smtpClient.UseDefaultCredentials = mailDefaultCredentials
            End If
            smtpClient.Send(myMail)
        Catch ex As Exception
        End Try
    End Sub

    Public Sub NewOrder_Proforma(headerId As String)
        If String.IsNullOrEmpty(headerId) Then Exit Sub

        Dim previewClass As New PreviewClass
        Dim pdfBytes As Byte() = previewClass.BindContent(headerId)

        Dim orderData As DataRow = GetDataRow("SELECT OrderHeaders.*, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId, Customers.Operator AS Operator FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id='" & headerId & "'")
        If orderData Is Nothing Then Exit Sub

        Dim customerId As String = orderData("CustomerId").ToString()
        Dim orderId As String = orderData("OrderId").ToString()
        Dim orderNumber As String = orderData("OrderNumber").ToString()
        Dim orderName As String = orderData("OrderName").ToString()
        Dim orderNote As String = orderData("OrderNote").ToString()

        Dim safeNote As String = HttpUtility.HtmlEncode(orderNote)
        safeNote = safeNote.Replace(vbCrLf, "<br>").Replace(vbLf, "<br>")

        Dim customerName As String = orderData("CustomerName").ToString()

        Dim companyId As String = orderData("CompanyId").ToString()
        Dim companyName As String = GetItemData("SELECT Name FROM Companys WHERE Id='" & companyId & "'")

        Dim mailData As DataRow = GetDataRow("SELECT * FROM Mailings WHERE CompanyId='" & companyId & "' AND Name='New Order | Proforma' AND Active=1")
        If mailData Is Nothing Then Exit Sub

        If Not companyId = "2" Then Exit Sub

        Dim mailServer As String = mailData("Server").ToString()
        Dim mailHost As String = mailData("Host").ToString()
        Dim mailPort As Integer = mailData("Port")

        Dim mailAccount As String = mailData("Account").ToString()
        Dim mailPassword As String = mailData("Password").ToString()
        Dim mailAlias As String = mailData("Alias").ToString()
        Dim mailSubject As String = mailData("Subject").ToString()

        Dim mailTo As String = mailData("To").ToString()
        Dim mailCc As String = mailData("Cc").ToString()
        Dim mailBcc As String = mailData("Bcc").ToString()

        Dim mailNetworkCredentials As Boolean = mailData("NetworkCredentials")
        Dim mailDefaultCredentials As Boolean = mailData("DefaultCredentials")
        Dim mailEnableSSL As Boolean = mailData("EnableSSL")

        Dim mailBody As String = String.Empty

        mailBody = "<span style='font-family: Cambria; font-size: 16px;'>"
        mailBody &= "Hi Bella & Team,"
        mailBody &= "<br /><br />"
        mailBody &= "A new proforma order has been submitted by the customer."
        mailBody &= "<br />"
        mailBody &= "Please log in to the web order portal to review the order and proceed with invoice issuance."
        mailBody &= "</span>"

        mailBody &= "<br /><br /><br />"

        mailBody &= "<table cellpadding='3' cellspacing='0' style='font-family:Cambria; font-size: 16px;'>"
        mailBody &= "<tr><td valign='top'>Company</td><td valign='top'>:</td><td valign='top'><b>" & companyName & "</b></td></tr>"
        mailBody &= "<tr><td valign='top'>Customer Name</td><td valign='top'>:</td><td valign='top'><b>" & customerName & "</b></td></tr>"
        mailBody &= "<tr><td valign='top'>Order #</td><td valign='top'>:</td><td valign='top'><b>" & orderId & "</b></td></tr>"
        mailBody &= "<tr><td valign='top'>Order Number</td><td valign='top'>:</td><td valign='top'><b>" & orderNumber & "</b></td></tr>"
        mailBody &= "<tr><td valign='top'>Order Name</td><td valign='top'>:</td><td valign='top'><b>" & orderName & "</b></td></tr>"
        mailBody &= "<tr><td valign='top'>Order Note</td><td valign='top'>:</td><td valign='top'><b>" & safeNote & "</b></td></tr>"
        mailBody &= "</table>"

        mailBody &= "<br /><br /><br />"

        mailBody &= "<span style='font-family: Cambria; font-size:16px;'>Kind Regards,</span>"
        mailBody &= "<br /><br /><br />"
        mailBody &= "<span style='font-family: Cambria; font-size:16px; font-weight: bold;'>Reza Andika Pratama</span><span style='font-family: Cambria; font-size:16px;'> | Developer</span>"
        mailBody &= "<br />"
        mailBody &= "<span style='font-family: Cambria; font-size:16px; font-weight: bold; color: red;'>E</span><span style='font-family: Cambria; font-size:16px;'> : reza@bigblinds.co.id</span>"
        mailBody &= "<br />"
        mailBody &= "<span style='font-family: Cambria; font-size:16px; font-weight: bold; color: red;'>P</span><span style='font-family: Cambria; font-size:16px;'> : +62 852 1504 3355</span>"

        Dim myMail As New MailMessage

        Dim subject As String = String.Format("{0} - {1} - {2} - New Proforma Order # {3}", customerName, orderNumber, orderName, orderId)

        myMail.Subject = String.Format(subject)
        myMail.From = New MailAddress(mailServer, mailAlias)

        If Not mailTo = "" Then
            Dim thisArray() As String = mailTo.Split(";")
            Dim thisMail As String = String.Empty
            For Each thisMail In thisArray
                myMail.To.Add(thisMail)
            Next
        End If

        If Not mailCc = "" Then
            Dim thisArray() As String = mailCc.Split(";")
            Dim thisMail As String = String.Empty
            For Each thisMail In thisArray
                myMail.CC.Add(thisMail)
            Next
        End If

        If Not mailBcc = "" Then
            Dim thisArray() As String = mailBcc.Split(";")
            Dim thisMail As String = String.Empty
            For Each thisMail In thisArray
                myMail.Bcc.Add(thisMail)
            Next
        End If

        myMail.Body = mailBody
        myMail.IsBodyHtml = True

        Dim ms As New MemoryStream(pdfBytes)
        Dim attach As New Attachment(ms, "Order_" & orderId & ".pdf", "application/pdf")
        myMail.Attachments.Add(attach)

        Dim smtpClient As New SmtpClient()
        smtpClient.Host = mailHost
        smtpClient.Port = mailPort
        smtpClient.EnableSsl = mailEnableSSL
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
        smtpClient.Timeout = 120000

        If mailNetworkCredentials Then
            smtpClient.UseDefaultCredentials = False
            smtpClient.Credentials = New NetworkCredential(mailAccount, mailPassword)
        Else
            smtpClient.UseDefaultCredentials = mailDefaultCredentials
        End If
        smtpClient.Send(myMail)
    End Sub

    Public Sub NewOrder_PrintingFabric(headerId As String)
        If String.IsNullOrEmpty(headerId) Then Exit Sub

        Dim previewClass As New PreviewClass
        Dim pdfBytes As Byte() = previewClass.BindContent(headerId)

        Dim orderData As DataRow = GetDataRow("SELECT OrderHeaders.*, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id='" & headerId & "'")
        If orderData Is Nothing Then Exit Sub

        Dim customerId As String = orderData("CustomerId").ToString()
        Dim orderId As String = orderData("OrderId").ToString()
        Dim orderNumber As String = orderData("OrderNumber").ToString()
        Dim orderName As String = orderData("OrderName").ToString()
        Dim orderNote As String = orderData("OrderNote").ToString()
        Dim orderStatus As String = orderData("Status").ToString()

        Dim customerName As String = orderData("CustomerName").ToString()
        Dim companyId As String = orderData("CompanyId").ToString()
        Dim companyName As String = GetItemData("SELECT Name FROM Companys WHERE Id='" & companyId & "'")

        Dim mailData As DataRow = GetDataRow("SELECT * FROM Mailings WHERE CompanyId='" & companyId & "' AND Name='Printing Fabric' AND Active=1")
        If mailData Is Nothing Then Exit Sub

        Dim mailServer As String = mailData("Server").ToString()
        Dim mailHost As String = mailData("Host").ToString()
        Dim mailPort As Integer = mailData("Port")

        Dim mailAccount As String = mailData("Account").ToString()
        Dim mailPassword As String = mailData("Password").ToString()
        Dim mailAlias As String = mailData("Alias").ToString()
        Dim mailSubject As String = mailData("Subject").ToString()

        Dim mailTo As String = mailData("To").ToString()
        Dim mailCc As String = mailData("Cc").ToString()
        Dim mailBcc As String = mailData("Bcc").ToString()

        Dim mailNetworkCredentials As Boolean = mailData("NetworkCredentials")
        Dim mailDefaultCredentials As Boolean = mailData("DefaultCredentials")
        Dim mailEnableSSL As Boolean = mailData("EnableSSL")

        Dim mailBody As String = String.Empty

        mailBody = "<span style='font-family: Cambria; font-size: 16px;'>"
        mailBody &= "Hi <b>Tatang</b>,"
        mailBody &= "<br /><br />"
        mailBody &= "A new order has been submitted by the customer, including printed fabric."
        mailBody &= "<br />"
        mailBody &= "Please access the web portal and download the images uploaded by the customer."
        mailBody &= "<br />"
        mailBody &= "Navigation:"
        mailBody &= "<br />"
        mailBody &= "<b>Order → File Order → Enter this <u>Order #</u> in the search field.</b>"

        If orderStatus = "Waiting Proforma" Then
            mailBody &= "<br /><br />"
            mailBody &= "This is a <b>PROFORMA</b> customer."
            mailBody &= "<br />"
            mailBody &= "Please coordinate with the Account Team before proceeding with the process."
        End If
        mailBody &= "</span>"

        mailBody &= "<br /><br /><br />"

        mailBody &= "<table cellpadding='3' cellspacing='0' style='font-family:Cambria; font-size: 16px;'>"
        mailBody &= "<tr><td valign='top'>Company</td><td valign='top'>:</td><td valign='top'><b>" & companyName & "</b></td></tr>"
        mailBody &= "<tr><td valign='top'>Customer Name</td><td valign='top'>:</td><td valign='top'><b>" & customerName & "</b></td></tr>"
        mailBody &= "<tr><td valign='top'>Order #</td><td valign='top'>:</td><td valign='top'><b>" & orderId & "</b></td></tr>"
        mailBody &= "<tr><td valign='top'>Order Number</td><td valign='top'>:</td><td valign='top'><b>" & orderNumber & "</b></td></tr>"
        mailBody &= "<tr><td valign='top'>Order Name</td><td valign='top':</td><td valign='top'><b>" & orderName & "</b></td></tr>"
        mailBody &= "<tr><td valign='top'>Order Note</td><td valign='top'>:</td><td valign='top'><b>" & orderNote & "</b></td></tr>"
        mailBody &= "</table>"

        mailBody &= "<br /><br /><br />"

        mailBody &= "<span style='font-family: Cambria; font-size:16px;'>Kind Regards,</span>"
        mailBody &= "<br /><br /><br />"
        mailBody &= "<span style='font-family: Cambria; font-size:16px;font-weight: bold;'>Reza Andika Pratama</span><span style='font-family: Cambria; font-size:16px;'> | Developer</span>"
        mailBody &= "<br />"
        mailBody &= "<span style='font-family: Cambria; font-size:16px;color:red;'>E</span><span style='font-family: Cambria; font-size:16px;'> : reza@bigblinds.co.id</span>"
        mailBody &= "<br />"
        mailBody &= "<span style='font-family: Cambria; font-size:16px;color:red;'>P</span><span style='font-family: Cambria; font-size:16px;'> : 0852-1504-3355</span>"

        Dim myMail As New MailMessage

        Dim subject As String = String.Format("{0} - {1} - {2} - New Order # {3}", customerName, orderNumber, orderName, orderId)

        myMail.Subject = subject
        myMail.From = New MailAddress(mailServer, mailAlias)

        If Not mailTo = "" Then
            Dim thisArray() As String = mailTo.Split(";")
            Dim thisMail As String = String.Empty
            For Each thisMail In thisArray
                myMail.To.Add(thisMail)
            Next
        End If

        If Not mailCc = "" Then
            Dim thisArray() As String = mailCc.Split(";")
            Dim thisMail As String = String.Empty
            For Each thisMail In thisArray
                myMail.CC.Add(thisMail)
            Next
        End If

        If Not mailBcc = "" Then
            Dim thisArray() As String = mailBcc.Split(";")
            Dim thisMail As String = String.Empty
            For Each thisMail In thisArray
                myMail.Bcc.Add(thisMail)
            Next
        End If

        myMail.Body = mailBody
        myMail.IsBodyHtml = True

        Dim ms As New MemoryStream(pdfBytes)
        Dim attach As New Attachment(ms, "Order_" & orderId & ".pdf", "application/pdf")
        myMail.Attachments.Add(attach)

        Dim smtpClient As New SmtpClient()
        smtpClient.Host = mailHost
        smtpClient.Port = mailPort
        smtpClient.EnableSsl = mailEnableSSL
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
        smtpClient.Timeout = 120000

        If mailNetworkCredentials Then
            smtpClient.UseDefaultCredentials = False
            smtpClient.Credentials = New NetworkCredential(mailAccount, mailPassword)
        Else
            smtpClient.UseDefaultCredentials = mailDefaultCredentials
        End If
        smtpClient.Send(myMail)
    End Sub

    Public Sub ProductionOrder(headerId As String)
        If String.IsNullOrEmpty(headerId) Then Exit Sub

        Dim orderData As DataRow = GetDataRow("SELECT OrderHeaders.*, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Id='" & headerId & "'")
        If orderData Is Nothing Then Exit Sub

        Dim customerId As String = orderData("CustomerId").ToString()
        Dim orderId As String = orderData("OrderId").ToString()
        Dim orderNumber As String = orderData("OrderNumber").ToString()
        Dim orderName As String = orderData("OrderName").ToString()
        Dim orderNote As String = orderData("OrderNote").ToString()
        Dim orderCreated As String = orderData("CreatedBy").ToString()

        Dim safeNote As String = HttpUtility.HtmlEncode(orderNote)
        safeNote = safeNote.Replace(vbCrLf, "<br>").Replace(vbLf, "<br>")

        Dim customerName As String = orderData("CustomerName").ToString()
        Dim companyId As String = orderData("CompanyId").ToString()
        Dim companyName As String = GetItemData("SELECT Name FROM Companys WHERE Id='" & companyId & "'")

        Dim createdMail As String = GetItemData("SELECT Email FROM CustomerLogins WHERE Id='" & orderCreated & "'")
        Dim createdRole As String = GetItemData("SELECT RoleId FROM CustomerLogins WHERE Id='" & orderCreated & "'")
        Dim createdLevel As String = GetItemData("SELECT LevelId FROM CustomerLogins WHERE Id='" & orderCreated & "'")

        Dim mailData As DataRow = GetDataRow("SELECT * FROM Mailings WHERE CompanyId='" & companyId & "' AND Name='Production Order' AND Active=1")
        If mailData Is Nothing Then Exit Sub

        Dim mailServer As String = mailData("Server").ToString()
        Dim mailHost As String = mailData("Host").ToString()
        Dim mailPort As Integer = mailData("Port")

        Dim mailAccount As String = mailData("Account").ToString()
        Dim mailPassword As String = mailData("Password").ToString()
        Dim mailAlias As String = mailData("Alias").ToString()
        Dim mailSubject As String = mailData("Subject").ToString()

        Dim mailTo As String = mailData("To").ToString()
        Dim mailCc As String = mailData("Cc").ToString()
        Dim mailBcc As String = mailData("Bcc").ToString()

        Dim mailNetworkCredentials As Boolean = mailData("NetworkCredentials")
        Dim mailDefaultCredentials As Boolean = mailData("DefaultCredentials")
        Dim mailEnableSSL As Boolean = mailData("EnableSSL")

        Dim mailBody As String = String.Empty

        mailBody = "<span style='font-family: Cambria; font-size: 16px;'>"
        mailBody &= "Thank you for your order."
        mailBody &= "<br /><br />"
        mailBody &= "This is an automated message confirming the receipt of your order."
        mailBody &= "<br /><br />"
        mailBody &= "The order below has been successfully registered and has been forwarded directly to our production system for processing."
        mailBody &= "<br />"
        mailBody &= "Please note that due to this streamlined process, we regret to inform you that we are unable to accept cancellations or modifications for this order."
        mailBody &= "</span>"

        mailBody &= "<span style='font-family: Cambria; font-size: 16px;'>"
        mailBody &= "For any inquiries or assistance, kindly contact our office. We appreciate your understanding and trust in our products & services."
        mailBody &= "</span>"

        mailBody &= "<br /><br /><br />"

        mailBody &= "<span style='font-family: Cambria; font-size:16px;'>Kind Regards,</span>"
        mailBody &= "<br /><br />"
        mailBody &= "<span style='font-family: Cambria; font-size:16px; font-weight: bold;'>" & companyName & "</span>"

        Dim myMail As New MailMessage

        Dim subject As String = String.Format("{0} - {1} - {2} - New Order # {3}", customerName, orderNumber, orderName, orderId)
        myMail.Subject = subject
        myMail.From = New MailAddress(mailServer, mailAlias)

        If createdRole = "8" AndAlso Not String.IsNullOrEmpty(createdMail) Then
            myMail.To.Add(createdMail)
        End If

        Dim customerMail As String = GetItemData("SELECT Email FROM CustomerContacts WHERE CustomerId='" & customerId & "' AND [Primary]=1")
        If Not String.IsNullOrEmpty(customerMail) Then
            If Not customerMail = createdMail Then
                myMail.To.Add(customerMail)
            End If
        End If

        If myMail.To.Count = 0 Then Exit Sub

        Dim ccSet As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        Dim customerMail_CC As DataTable = GetDataTable("SELECT Email FROM CustomerContacts CROSS APPLY STRING_SPLIT(Tags, ',') AS thisArray WHERE CustomerId='" & customerId & "' AND thisArray.VALUE='Confirming' AND LTRIM(RTRIM(Email)) <> '' AND Email IS NOT NULL AND [Primary]=0")
        If customerMail_CC.Rows.Count > 0 Then
            For Each row As DataRow In customerMail_CC.Rows
                ccSet.Add(row("Email").ToString().Trim())
            Next
        End If

        If Not String.IsNullOrWhiteSpace(mailCc) Then
            For Each email As String In mailCc.Split(";"c)
                ccSet.Add(email.Trim())
            Next
        End If

        For Each ccEmail As String In ccSet
            If Not String.IsNullOrWhiteSpace(ccEmail) Then
                myMail.CC.Add(ccEmail)
            End If
        Next

        If Not String.IsNullOrWhiteSpace(mailBcc) Then
            For Each thisMail As String In mailBcc.Split(";"c)
                If Not String.IsNullOrWhiteSpace(thisMail) Then
                    myMail.Bcc.Add(thisMail.Trim())
                End If
            Next
        End If

        myMail.Body = mailBody
        myMail.IsBodyHtml = True

        Dim smtpClient As New SmtpClient()
        smtpClient.Host = mailHost
        smtpClient.Port = mailPort
        smtpClient.EnableSsl = mailEnableSSL
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
        smtpClient.Timeout = 120000

        If mailNetworkCredentials Then
            smtpClient.UseDefaultCredentials = False
            smtpClient.Credentials = New NetworkCredential(mailAccount, mailPassword)
        Else
            smtpClient.UseDefaultCredentials = mailDefaultCredentials
        End If

        smtpClient.Send(myMail)
    End Sub

    Public Sub SentQuote(headerId As String, actionBy As String, toCust As String, ccCust As String, ccStaff As String)
        If String.IsNullOrEmpty(headerId) Then Exit Sub

        Dim previewClass As New PreviewClass
        Dim quoteClass As New QuoteClass

        Dim pdfOrder As Byte() = previewClass.BindContent(headerId)
        Dim pdfQuote As Byte() = quoteClass.BindContent(headerId)

        Dim orderData As DataRow = GetDataRow("SELECT OrderHeaders.*, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId, Customers.Operator AS Operator, OrderInvoices.InvoiceNumber AS InvoiceNumber FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id LEFT JOIN OrderInvoices ON OrderHeaders.Id=OrderInvoices.Id WHERE OrderHeaders.Id='" & headerId & "'")
        If orderData Is Nothing Then Exit Sub

        Dim customerId As String = orderData("CustomerId").ToString()
        Dim orderId As String = orderData("OrderId").ToString()
        Dim orderNumber As String = orderData("OrderNumber").ToString()
        Dim orderName As String = orderData("OrderName").ToString()

        Dim customerName As String = orderData("CustomerName").ToString()
        Dim customerOperator As String = orderData("Operator").ToString()

        Dim companyId As String = orderData("CompanyId").ToString()
        Dim companyName As String = GetItemData("SELECT Name FROM Companys WHERE Id='" & companyId & "'")

        Dim operatorEmail As String = GetItemData("SELECT Email FROM CustomerLogins WHERE Id='" & customerOperator & "'")

        Dim mailData As DataRow = GetDataRow("SELECT * FROM Mailings WHERE CompanyId='" & companyId & "' AND Name='Send Quote' AND Active=1")
        If mailData Is Nothing Then Exit Sub

        Dim mailServer As String = mailData("Server").ToString().ToString()
        Dim mailHost As String = mailData("Host").ToString()
        Dim mailPort As Integer = mailData("Port")

        Dim mailAccount As String = mailData("Account").ToString()
        Dim mailPassword As String = mailData("Password").ToString()
        Dim mailAlias As String = mailData("Alias").ToString()
        Dim mailSubject As String = mailData("Subject").ToString()

        Dim mailTo As String = mailData("To").ToString()
        Dim mailCc As String = mailData("Cc").ToString()
        Dim mailBcc As String = mailData("Bcc").ToString()

        Dim mailNetworkCredentials As Boolean = mailData("NetworkCredentials")
        Dim mailDefaultCredentials As Boolean = mailData("DefaultCredentials")
        Dim mailEnableSSL As Boolean = mailData("EnableSSL")

        Dim actionName As String = GetItemData("SELECT FullName FROM CustomerLogins WHERE Id='" & actionBy & "'")
        Dim actionEmail As String = GetItemData("SELECT Email FROM CustomerLogins WHERE Id='" & actionBy & "'")
        Dim actionRole As String = GetItemData("SELECT LoginRoles.Name FROM CustomerLogins LEFT JOIN LoginRoles ON CustomerLogins.RoleId=LoginRoles.Id WHERE CustomerLogins.Id='" & actionBy & "'")

        Dim signatureUser As String = String.Format("{0} - {1}", actionName, actionRole)

        Dim mailBody As String = String.Empty

        mailBody = "<span style='font-family: Cambria; font-size: 16px;'>"
        mailBody &= "Hi <b>" & customerName & "</b>,"
        mailBody &= "<br /><br />"
        mailBody &= "Please find attached the quotation for your review."
        mailBody &= "<br /><br />"
        mailBody &= "Should you have any questions or require further information, please do not hesitate to contact us."
        mailBody &= "<br /><br />"
        mailBody &= "Thank you for your attention."
        mailBody &= "</span>"

        mailBody &= "<br /><br /><br />"

        mailBody &= "<span style='font-family: Cambria; font-size:16px;'>Kind Regards,</span>"
        mailBody &= "<br /><br />"
        If companyId = "2" Then
            mailBody &= "<span style='font-family: Cambria; font-size:16px; font-weight: bold;'>" & signatureUser & "</span>"
            mailBody &= "<br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px;'>" & actionEmail & "</span>"
            mailBody &= "<br /><br /><br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px; font-weight: bold;'>" & companyName & "</span>"
            mailBody &= "<br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px; color:red;'>Phone : </span><span style='font-family: Cambria; font-size:16px;'>0417 705 109</span>"
            mailBody &= "<br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px; color:red;'>Email : </span><span style='font-family: Cambria; font-size:16px;'>order@jpmdirect.com.au</span>"
            mailBody &= "<br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px; color:red;'>Website : </span><span style='font-family: Cambria; font-size:16px;'>http://jpmdirect.com.au/</span>"
        End If

        Dim myMail As New MailMessage()

        Dim subject As String = String.Format("{0} - {1} - {2} - Quote Order # {3}", customerName, orderNumber, orderName, orderId)
        myMail.Subject = subject
        myMail.From = New MailAddress(mailServer, mailAlias)

        myMail.To.Add(toCust)
        If Not String.IsNullOrEmpty(ccCust) Then myMail.CC.Add(ccCust)

        If Not String.IsNullOrEmpty(mailCc) Then
            For Each thisMail In mailCc.Split(";"c)
                If Not String.IsNullOrEmpty(thisMail.Trim()) Then myMail.CC.Add(thisMail.Trim())
            Next
        End If
        If Not String.IsNullOrEmpty(operatorEmail) Then myMail.CC.Add(operatorEmail)
        If Not String.IsNullOrEmpty(ccStaff) Then myMail.CC.Add(ccStaff)

        If Not String.IsNullOrEmpty(mailBcc) Then
            For Each thisMail In mailBcc.Split(";"c)
                If Not String.IsNullOrEmpty(thisMail.Trim()) Then myMail.Bcc.Add(thisMail.Trim())
            Next
        End If

        myMail.IsBodyHtml = True
        myMail.Body = mailBody

        myMail.Attachments.Add(New Attachment(New MemoryStream(pdfOrder),
                                      "ORDER-" & orderId & ".pdf",
                                      "application/pdf"))

        myMail.Attachments.Add(New Attachment(New MemoryStream(pdfQuote),
                                      "QUOTE-" & orderId & ".pdf",
                                      "application/pdf"))

        Dim smtpClient As New SmtpClient()
        smtpClient.Host = mailHost
        smtpClient.Port = mailPort
        smtpClient.EnableSsl = mailEnableSSL
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
        smtpClient.Timeout = 120000

        If mailNetworkCredentials Then
            smtpClient.UseDefaultCredentials = False
            smtpClient.Credentials = New NetworkCredential(mailAccount, mailPassword)
        Else
            smtpClient.UseDefaultCredentials = mailDefaultCredentials
        End If

        smtpClient.Send(myMail)
    End Sub

    Public Sub SendInvoice(headerId As String, actionBy As String, toCust As String, ccCust As String, ccStaff As String)
        If String.IsNullOrEmpty(headerId) Then Exit Sub

        Dim previewClass As New PreviewClass
        Dim invoiceClass As New InvoiceClass

        Dim pdfOrder As Byte() = previewClass.BindContent(headerId)
        Dim pdfInvoice As Byte() = invoiceClass.BindContent(headerId)

        Dim orderData As DataRow = GetDataRow("SELECT OrderHeaders.*, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId, Customers.Operator AS Operator, OrderInvoices.InvoiceNumber AS InvoiceNumber FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id LEFT JOIN OrderInvoices ON OrderHeaders.Id=OrderInvoices.Id WHERE OrderHeaders.Id='" & headerId & "'")
        If orderData Is Nothing Then Exit Sub

        Dim customerId As String = orderData("CustomerId").ToString()
        Dim orderId As String = orderData("OrderId").ToString()
        Dim orderNumber As String = orderData("OrderNumber").ToString()
        Dim orderName As String = orderData("OrderName").ToString()
        Dim invoiceNumber As String = orderData("InvoiceNumber").ToString()

        Dim customerName As String = orderData("CustomerName").ToString()
        Dim customerOperator As String = orderData("Operator").ToString()

        Dim companyId As String = orderData("CompanyId").ToString()
        Dim companyName As String = GetItemData("SELECT Name FROM Companys WHERE Id='" & companyId & "'")

        Dim operatorEmail As String = GetItemData("SELECT Email FROM CustomerLogins WHERE Id='" & customerOperator & "'")

        Dim mailData As DataRow = GetDataRow("SELECT * FROM Mailings WHERE CompanyId='" & companyId & "' AND Name='Send Invoice' AND Active=1")
        If mailData Is Nothing Then Exit Sub

        Dim mailServer As String = mailData("Server").ToString()
        Dim mailHost As String = mailData("Host").ToString()
        Dim mailPort As Integer = mailData("Port")

        Dim mailAccount As String = mailData("Account").ToString()
        Dim mailPassword As String = mailData("Password").ToString()
        Dim mailAlias As String = mailData("Alias").ToString()
        Dim mailSubject As String = mailData("Subject").ToString()

        Dim mailTo As String = mailData("To").ToString()
        Dim mailCc As String = mailData("Cc").ToString()
        Dim mailBcc As String = mailData("Bcc").ToString()

        Dim mailNetworkCredentials As Boolean = mailData("NetworkCredentials")
        Dim mailDefaultCredentials As Boolean = mailData("DefaultCredentials")
        Dim mailEnableSSL As Boolean = mailData("EnableSSL")

        Dim actionName As String = GetItemData("SELECT FullName FROM CustomerLogins WHERE Id='" & actionBy & "'")
        Dim actionEmail As String = GetItemData("SELECT Email FROM CustomerLogins WHERE Id='" & actionBy & "'")
        Dim actionRole As String = GetItemData("SELECT LoginRoles.Name FROM CustomerLogins LEFT JOIN LoginRoles ON CustomerLogins.RoleId=LoginRoles.Id WHERE CustomerLogins.Id='" & actionBy & "'")

        Dim signatureUser As String = String.Format("{0} - {1}", actionName, actionRole)

        Dim mailBody As String = String.Empty

        mailBody = "<span style='font-family: Cambria; font-size: 16px;'>"
        mailBody &= "Dear Valued Customer,"
        mailBody &= "<br /><br />"
        mailBody &= "Thank you for placing your order with us."
        mailBody &= "<br /><br />"
        mailBody &= "Please find attached the payment invoice for your order. We kindly ask you to proceed with the payment in accordance with the details provided in the invoice."
        mailBody &= "<br /><br />"

        mailBody &= "If you would like to make payment by credit card, please refer to the link below:"
        mailBody &= "<br />"
        mailBody &= "<a href='https://pay.b2bpay.com.au/JPMDi'>https://pay.b2bpay.com.au/JPMDi</a>"

        mailBody &= "<br /><br />"
        mailBody &= "Once the payment has been completed, please send your payment confirmation to the following email address:"
        mailBody &= "<br />"
        mailBody &= "<a href='mailto:invoice@jpmdirect.com.au'>invoice@jpmdirect.com.au</a>"

        mailBody &= "<br /><br />"
        mailBody &= "After the payment has been successfully received and confirmed, your order will be processed accordingly."
        mailBody &= "<br /><br />"
        mailBody &= "Should you have any questions or require further assistance, please do not hesitate to contact us."
        mailBody &= "<br /><br />"
        mailBody &= "Thank you for your trust and cooperation."
        mailBody &= "</span>"

        mailBody &= "<br /><br /><br />"

        mailBody &= "<span style='font-family: Cambria; font-size:16px;'>Kind Regards,</span>"
        mailBody &= "<br />"
        If companyId = "2" Then
            mailBody &= "<span style='font-family: Cambria; font-size:16px; font-weight: bold;'>" & signatureUser & "</span>"
            mailBody &= "<br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px;'>" & actionEmail & "</span>"
            mailBody &= "<br /><br /><br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px; font-weight: bold;'>" & companyName & "</span>"
            mailBody &= "<br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px; color:red;'>P : </span><span style='font-family: Cambria; font-size:16px;'>0417 705 109</span>"
            mailBody &= "<br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px; color:red;'>E : </span><span style='font-family: Cambria; font-size:16px;'>order@jpmdirect.com.au</span>"
            mailBody &= "<br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px; color:red;'>W : </span><span style='font-family: Cambria; font-size:16px;'>http://jpmdirect.com.au/</span>"
        End If

        Dim myMail As New MailMessage()

        Dim subject As String = String.Format("{0} - {1} - {2} - Invoice Order # {3}", customerName, orderNumber, orderName, orderId)

        myMail.Subject = subject
        myMail.From = New MailAddress(mailServer, mailAlias)

        myMail.To.Add(toCust)
        If Not String.IsNullOrEmpty(ccCust) Then myMail.CC.Add(ccCust)

        If Not String.IsNullOrEmpty(mailCc) Then
            For Each thisMail In mailCc.Split(";"c)
                If Not String.IsNullOrEmpty(thisMail.Trim()) Then myMail.CC.Add(thisMail.Trim())
            Next
        End If
        If Not String.IsNullOrEmpty(operatorEmail) Then myMail.CC.Add(operatorEmail)
        If Not String.IsNullOrEmpty(ccStaff) Then myMail.CC.Add(ccStaff)

        If Not String.IsNullOrEmpty(mailBcc) Then
            For Each thisMail In mailBcc.Split(";"c)
                If Not String.IsNullOrEmpty(thisMail.Trim()) Then myMail.Bcc.Add(thisMail.Trim())
            Next
        End If

        myMail.IsBodyHtml = True
        myMail.Body = mailBody

        myMail.Attachments.Add(New Attachment(New MemoryStream(pdfOrder),
                                      "ORDER-" & orderId & ".pdf",
                                      "application/pdf"))

        myMail.Attachments.Add(New Attachment(New MemoryStream(pdfInvoice),
                                      invoiceNumber & ".pdf",
                                      "application/pdf"))

        Dim smtpClient As New SmtpClient()
        smtpClient.Host = mailHost
        smtpClient.Port = mailPort
        smtpClient.EnableSsl = mailEnableSSL
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
        smtpClient.Timeout = 120000

        If mailNetworkCredentials Then
            smtpClient.UseDefaultCredentials = False
            smtpClient.Credentials = New NetworkCredential(mailAccount, mailPassword)
        Else
            smtpClient.UseDefaultCredentials = mailDefaultCredentials
        End If

        smtpClient.Send(myMail)
    End Sub



    Public Sub ReworkOrder(reworkId As String, fileDirectory As String)
        If String.IsNullOrEmpty(reworkId) OrElse String.IsNullOrEmpty(fileDirectory) Then Exit Sub

        Dim reworkData As DataRow = GetDataRow("SELECT OrderReworks.*, CustomerLogins.FullName AS CreatedFullName, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId, OrderHeaders.OrderId AS OrderId, OrderHeaders.OrderNumber AS OrderNumber, OrderHeaders.OrderName AS OrderName, OrderHeaders.CustomerId AS CustomerId, OrderHeaders.OrderId FROM OrderReworks LEFT JOIN OrderHeaders ON OrderReworks.HeaderId=OrderHeaders.Id LEFT JOIN CustomerLogins ON OrderReworks.CreatedBy=CustomerLogins.Id LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderReworks.Id='" & reworkId & "'")
        If reworkData Is Nothing Then Exit Sub

        Dim customerId As String = reworkData("CustomerId").ToString()
        Dim customerName As String = reworkData("CustomerName").ToString()
        Dim orderId As String = reworkData("OrderId").ToString()
        Dim orderNumber As String = reworkData("OrderNumber").ToString()
        Dim orderName As String = reworkData("OrderName").ToString()
        Dim requestedBy As String = reworkData("CreatedFullName").ToString()

        Dim companyId As String = GetItemData("SELECT CompanyId FROM Customers WHERE Id='" & customerId & "'")
        Dim companyName As String = GetItemData("SELECT Name FROM Companys WHERE Id='" & companyId & "'")

        Dim mailData As DataRow = GetDataRow("SELECT * FROM Mailings WHERE CompanyId='" & companyId & "' AND Name='Rework Order' AND Active=1")
        If mailData Is Nothing Then Exit Sub

        Dim mailServer As String = mailData("Server").ToString()
        Dim mailHost As String = mailData("Host").ToString()
        Dim mailPort As Integer = mailData("Port")

        Dim mailAccount As String = mailData("Account").ToString()
        Dim mailPassword As String = mailData("Password").ToString()
        Dim mailAlias As String = mailData("Alias").ToString()
        Dim mailSubject As String = mailData("Subject").ToString()

        Dim mailTo As String = mailData("To").ToString()
        Dim mailCc As String = mailData("Cc").ToString()
        Dim mailBcc As String = mailData("Bcc").ToString()

        Dim mailNetworkCredentials As Boolean = mailData("NetworkCredentials")
        Dim mailDefaultCredentials As Boolean = mailData("DefaultCredentials")
        Dim mailEnableSSL As Boolean = mailData("EnableSSL")

        Dim mailBody As String = String.Empty

        mailBody = "<span style='font-family: Cambria; font-size: 16px;'>"
        mailBody &= "Hi Team,"
        mailBody &= "<br />"
        mailBody &= "We have just received a new rework request."
        mailBody &= "<br /><br />"
        mailBody &= "Rework Details:"
        mailBody &= "</span>"

        mailBody &= "<br /><br />"

        mailBody &= "<table cellpadding='3' cellspacing='0' style='font-family:Cambria; font-size: 16px;'>"
        mailBody &= "<tr><td valign='top'>Company</td><td valign='top'>:</td><td valign='top'><b>" & companyName & "</b></td></tr>"
        mailBody &= "<tr><td valign='top'>Customer Name</td><td valign='top'>:</td><td valign='top'><b>" & customerName & "</b></td></tr>"
        mailBody &= "<tr><td valign='top'>Order #</td><td valign='top'>:</td><td valign='top'><b>" & orderId & "</b></td></tr>"
        mailBody &= "<tr><td valign='top'>Order Number</td><td valign='top'>:</td><td valign='top'><b>" & orderNumber & "</b></td></tr>"
        mailBody &= "<tr><td valign='top'>Order Name</td><td valign='top'>:</td><td valign='top'><b>" & orderName & "</b></td></tr>"
        mailBody &= "<tr><td valign='top'>Requested By</td><td valign='top'>:</td><td valign='top'><b>" & requestedBy & "</b></td></tr>"
        mailBody &= "</table>"

        mailBody &= "<br /><br />"

        mailBody &= "<span style='font-family: Cambria; font-size: 16px;'>"
        mailBody &= "Please check it on the online ordering portal and proceed with the next step — either <b>Approve</b> or <b>Reject</b> the request."
        mailBody &= "</span>"

        mailBody &= "<br /><br /><br />"

        mailBody &= "<span style='font-family: Cambria; font-size:16px;'>Kind Regards,</span>"
        mailBody &= "<br /><br /><br />"
        mailBody &= "<span style='font-family: Cambria; font-size:16px;font-weight: bold;'>Reza Andika Pratama</span><span style='font-family: Cambria; font-size:16px;'> | Developer</span>"
        mailBody &= "<br />"
        mailBody &= "<span style='font-family: Cambria; font-size:16px;color:red;'>E</span><span style='font-family: Cambria; font-size:16px;'> : reza@bigblinds.co.id</span>"
        mailBody &= "<br />"
        mailBody &= "<span style='font-family: Cambria; font-size:16px;color:red;'>P</span><span style='font-family: Cambria; font-size:16px;'> : 0852-1504-3355</span>"

        Dim myMail As New MailMessage

        Dim subject As String = String.Format("{0} - {1} - {2} - Rework Order # {3}", customerName, orderNumber, orderName, orderId)

        myMail.Subject = subject
        myMail.From = New MailAddress(mailServer, mailAlias)

        If Not mailTo = "" Then
            Dim thisArray() As String = mailTo.Split(";")
            Dim thisMail As String = String.Empty
            For Each thisMail In thisArray
                myMail.To.Add(thisMail)
            Next
        End If

        If Not mailCc = "" Then
            Dim thisArray() As String = mailCc.Split(";")
            Dim thisMail As String = String.Empty
            For Each thisMail In thisArray
                myMail.CC.Add(thisMail)
            Next
        End If

        If Not mailBcc = "" Then
            Dim thisArray() As String = mailBcc.Split(";")
            Dim thisMail As String = String.Empty
            For Each thisMail In thisArray
                myMail.Bcc.Add(thisMail)
            Next
        End If

        myMail.Body = mailBody
        myMail.IsBodyHtml = True
        myMail.Attachments.Add(New Attachment(fileDirectory))
        Dim smtpClient As New SmtpClient()
        smtpClient.Host = mailHost
        smtpClient.Port = mailPort
        smtpClient.EnableSsl = mailEnableSSL
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
        smtpClient.Timeout = 120000

        If mailNetworkCredentials Then
            smtpClient.UseDefaultCredentials = False
            smtpClient.Credentials = New NetworkCredential(mailAccount, mailPassword)
        Else
            smtpClient.UseDefaultCredentials = mailDefaultCredentials
        End If
        smtpClient.Send(myMail)
    End Sub

    Public Sub ReworkApprove(reworkId As String)
        If String.IsNullOrEmpty(reworkId) Then Exit Sub

        Dim orderData As DataRow = GetDataRow("SELECT OrderReworks.*, CustomerLogins.FullName AS CreatedFullName, Customers.Name AS CustomerName, Customers.CompanyId AS CompanyId, OrderHeaders.OrderId AS OrderId, OrderHeaders.OrderNumber AS OrderNumber, OrderHeaders.OrderName AS OrderName, OrderHeaders.CustomerId AS CustomerId, OrderHeaders.OrderId FROM OrderReworks LEFT JOIN OrderHeaders ON OrderReworks.HeaderId=OrderHeaders.Id LEFT JOIN CustomerLogins ON OrderReworks.CreatedBy=CustomerLogins.Id LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderReworks.Id='" & reworkId & "'")

        If orderData Is Nothing Then Exit Sub

        Dim customerId As String = orderData("CustomerId").ToString()
        Dim customerName As String = orderData("CustomerName").ToString()

        Dim orderId As String = orderData("OrderId").ToString()
        Dim orderNumber As String = orderData("OrderNumber").ToString()
        Dim orderName As String = orderData("OrderName").ToString()

        Dim headerIdNew As String = orderData("HeaderIdNew").ToString()

        Dim orderNewData As DataRow = GetDataRow("SELECT * FROM OrderHeaders WHERE Id='" & headerIdNew & "'")
        Dim orderIdNew As String = orderNewData("OrderId").ToString()
        Dim orderNumberNew As String = orderNewData("OrderNumber").ToString()
        Dim orderNameNew As String = orderNewData("OrderName").ToString()

        Dim companyId As String = orderNewData("CompanyId").ToString()
        Dim companyName As String = GetItemData("SELECT Name FROM Companys WHERE Id='" & companyId & "'")

        Dim mailData As DataRow = GetDataRow("SELECT * FROM Mailings WHERE CompanyId='" & companyId & "' AND Name='Rework Approved' AND Active=1")
        If mailData Is Nothing Then Exit Sub

        Dim mailServer As String = mailData("Server").ToString()
        Dim mailHost As String = mailData("Host").ToString()
        Dim mailPort As Integer = mailData("Port")

        Dim mailAccount As String = mailData("Account").ToString()
        Dim mailPassword As String = mailData("Password").ToString()
        Dim mailAlias As String = mailData("Alias").ToString()
        Dim mailSubject As String = mailData("Subject").ToString()

        Dim mailTo As String = mailData("To").ToString()
        Dim mailCc As String = mailData("Cc").ToString()
        Dim mailBcc As String = mailData("Bcc").ToString()

        Dim mailNetworkCredentials As Boolean = mailData("NetworkCredentials")
        Dim mailDefaultCredentials As Boolean = mailData("DefaultCredentials")
        Dim mailEnableSSL As Boolean = mailData("EnableSSL")

        Dim customerMail As DataTable = GetDataTable("SELECT Email FROM CustomerContacts CROSS APPLY STRING_SPLIT(Tags, ',') AS thisArray WHERE CustomerId='" & customerId & "' AND thisArray.VALUE='Confirming'")
        If customerMail.Rows.Count = 0 Then Exit Sub

        Dim mailBody As String = String.Empty

        mailBody = "<span style='font-family: Cambria; font-size: 16px;'>"
        mailBody &= String.Format("Hi <b>{0}</b>,", customerName)
        mailBody &= "<br /><br />"
        mailBody &= "The rework request you submitted has been approved for processing."
        mailBody &= "<br /><br />"
        mailBody &= "We have created a new job in the online ordering portal with the following details:"
        mailBody &= "</span>"

        mailBody &= "<br /><br />"

        mailBody &= "<table cellpadding='5' cellspacing='0' style='font-family:Cambria; font-size:16px; border-collapse:collapse; border:1px solid #000;'>"
        mailBody &= "<tr style='background-color:#f0f0f0;'>"
        mailBody &= "<td style='font-weight:bold; border:1px solid #000;'>Data</td>"
        mailBody &= "<td style='font-weight:bold; border:1px solid #000;'>Original Order</td>"
        mailBody &= "<td style='font-weight:bold; border:1px solid #000;'>New Order (Rework)</td>"
        mailBody &= "</tr>"

        mailBody &= "<tr><td style='border:1px solid #000;'>Customer Name</td><td style='border:1px solid #000;'>" & customerName & "</td><td style='border:1px solid #000;'>" & customerName & "</td></tr>"
        mailBody &= "<tr><td style='border:1px solid #000;'>Order ID</td><td style='border:1px solid #000;'>" & orderId & "</td><td style='border:1px solid #000;'>" & orderIdNew & "</td></tr>"
        mailBody &= "<tr><td style='border:1px solid #000;'>Order Number</td><td style='border:1px solid #000;'>" & orderNumber & "</td><td style='border:1px solid #000;'>" & orderNumberNew & "</td></tr>"
        mailBody &= "<tr><td style='border:1px solid #000;'>Order Name</td><td style='border:1px solid #000;'>" & orderName & "</td><td style='border:1px solid #000;'>" & orderNameNew & "</td></tr>"
        mailBody &= "</table>"

        mailBody &= "<br /><br />"

        mailBody &= "<span style='font-family: Cambria; font-size:16px;'>Kind Regards,</span>"
        mailBody &= "<br /><br /><br />"
        If companyId = "2" Then
            mailBody &= "<span style='font-family: Cambria; font-size:16px;'>Customer Service</span>"
            mailBody &= "<br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px; font-weight: bold;'>" & companyName & "</span>"
        End If
        If companyId = "3" Then
            mailBody &= "<span style='font-family: Cambria; font-size:16px;font-weight: bold;'>Reza Andika Pratama</span><span style='font-family: Cambria; font-size:16px;'> | Developer</span>"
            mailBody &= "<br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px;font-weight: bold;'>E</span><span style='font-family: Cambria; font-size:16px;'> : reza@bigblinds.co.id</span>"
            mailBody &= "<br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px;font-weight: bold;'>P</span><span style='font-family: Cambria; font-size:16px;'> : 0852-1504-3355</span>"
        End If

        Dim myMail As New MailMessage

        myMail.Subject = String.Format("{0} Original Order {1}", mailSubject, orderId)
        myMail.From = New MailAddress(mailServer, mailAlias)

        If customerMail.Rows.Count > 0 Then
            For i As Integer = 0 To customerMail.Rows.Count - 1
                Dim thisEmail As String = customerMail.Rows(i)("Email").ToString()
                myMail.To.Add(thisEmail)
            Next
        End If

        If Not mailCc = "" Then
            Dim thisArray() As String = mailCc.Split(";")
            Dim thisMail As String = String.Empty
            For Each thisMail In thisArray
                myMail.CC.Add(thisMail)
            Next
        End If

        If Not mailBcc = "" Then
            Dim thisArray() As String = mailBcc.Split(";")
            Dim thisMail As String = String.Empty
            For Each thisMail In thisArray
                myMail.Bcc.Add(thisMail)
            Next
        End If

        myMail.Body = mailBody
        myMail.IsBodyHtml = True
        Dim smtpClient As New SmtpClient()
        smtpClient.Host = mailHost
        smtpClient.Port = mailPort
        smtpClient.EnableSsl = mailEnableSSL
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
        smtpClient.Timeout = 120000

        If mailNetworkCredentials Then
            smtpClient.UseDefaultCredentials = False
            smtpClient.Credentials = New NetworkCredential(mailAccount, mailPassword)
        Else
            smtpClient.UseDefaultCredentials = mailDefaultCredentials
        End If
        smtpClient.Send(myMail)
    End Sub

    Public Sub TicketRemainder(issue As String)
        Try
            If String.IsNullOrEmpty(issue) Then Exit Sub

            Dim ticketData As DataTable = GetDataTable("SELECT * FROM Tickets WHERE Issue='" & issue & "' AND Status=1")

            If ticketData.Rows.Count > 0 Then
                Dim dataTicketId As New List(Of String)

                For i As Integer = 0 To ticketData.Rows.Count - 1

                    Dim ticketId As String = ticketData.Rows(i)("Id").ToString()

                    Dim ticketDetails As DataRow = GetDataRow("SELECT TOP 1 TicketDetails.*, CustomerLogins.RoleId AS RoleId FROM TicketDetails LEFT JOIN CustomerLogins ON TicketDetails.ReplyBy=CustomerLogins.Id WHERE TicketDetails.TicketId='" & ticketId & "' AND TicketDetails.CreatedDate < DATEADD(hour, -1, GETDATE()) AND CustomerLogins.RoleId='8' ORDER BY TicketDetails.CreatedDate DESC")

                    If ticketDetails IsNot Nothing AndAlso ticketDetails("RoleId").ToString() = "8" Then
                        dataTicketId.Add(ticketId)
                    End If

                Next
                Dim arrTicketId() As String = dataTicketId.ToArray()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub Ticket(ticketId As String)
        Try
            If String.IsNullOrEmpty(ticketId) Then Exit Sub

            Dim ticketData As DataRow = GetDataRow("SELECT * FROM Tickets WHERE Id='" & ticketId & "'")
            If ticketData Is Nothing Then Exit Sub

            Dim ticketCode As String = ticketData("TicketCode").ToString()
            Dim ticketIssue As String = ticketData("Issue").ToString()
            Dim ticketSubject As String = ticketData("Subject").ToString()
            Dim ticketMessage As String = ticketData("Message").ToString()
            Dim safeMessage As String = ticketMessage.Replace(vbCrLf, "<br>").Replace(vbLf, "<br>")

            Dim ticketBy As String = ticketData("CreatedBy").ToString()
            Dim createdName As String = GetItemData("SELECT FullName FROM CustomerLogins WHERE Id='" & ticketBy & "'")
            Dim customerName As String = GetItemData("SELECT Customers.Name AS CustomerName FROM CustomerLogins LEFT JOIN Customers ON CustomerLogins.CustomerId=Customers.Id WHERE CustomerLogins.Id='" & ticketBy & "'")
            Dim companyId As String = GetItemData("SELECT Customers.CompanyId FROM CustomerLogins LEFT JOIN Customers ON CustomerLogins.CustomerId=Customers.Id WHERE CustomerLogins.Id='" & ticketBy & "'")
            Dim companyName As String = GetItemData("SELECT Name FROM Companys WHERE Id='" & companyId & "'")

            Dim mailName As String = String.Empty
            If ticketIssue = "Web Issue" Then mailName = "Ticket Web"
            If ticketIssue = "Product Issue" Then mailName = "Ticket Product"
            If ticketIssue = "Pricing Issue" Then mailName = "Ticket Pricing"
            If ticketIssue = "Other Issue" Then mailName = "Ticket Other"

            If String.IsNullOrEmpty(mailName) Then Exit Sub

            Dim mailData As DataRow = GetDataRow("SELECT * FROM Mailings WHERE CompanyId='" & companyId & "' AND Name='" & mailName & "' AND Active=1")
            If mailData Is Nothing Then Exit Sub

            Dim mailServer As String = mailData("Server").ToString()
            Dim mailHost As String = mailData("Host").ToString()
            Dim mailPort As Integer = mailData("Port")

            Dim mailAccount As String = mailData("Account").ToString()
            Dim mailPassword As String = mailData("Password").ToString()
            Dim mailAlias As String = mailData("Alias").ToString()
            Dim mailSubject As String = mailData("Subject").ToString()

            Dim mailTo As String = mailData("To").ToString()
            Dim mailCc As String = mailData("Cc").ToString()
            Dim mailBcc As String = mailData("Bcc").ToString()

            Dim mailNetworkCredentials As Boolean = mailData("NetworkCredentials")
            Dim mailDefaultCredentials As Boolean = mailData("DefaultCredentials")
            Dim mailEnableSSL As Boolean = mailData("EnableSSL")

            Dim mailBody As String = String.Empty

            mailBody = "<span style='font-family: Cambria; font-size: 16px;'>"
            mailBody &= "Hi Team,"
            mailBody &= "<br /><br />"
            mailBody &= "A new ticket has been assigned to you with the following details:"
            mailBody &= "</span>"

            mailBody &= "<br /><br />"

            mailBody &= "<table cellpadding='3' cellspacing='0' style='font-family:Cambria; font-size: 16px;'>"
            mailBody &= "<tr><td valign='top'>Company</td><td valign='top'>:</td><td valign='top'><b>" & companyName & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Customer Name</td><td valign='top'>:</td><td valign='top'><b>" & customerName & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Created By</td><td valign='top'>:</td><td valign='top'><b>" & createdName & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Ticket Code</td><td valign='top'>:</td><td valign='top'><b>" & ticketCode & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Issue</td><td valign='top'>:</td><td valign='top'><b>" & ticketIssue & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Subject</td><td valign='top'>:</td><td valign='top'><b>" & ticketSubject & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Message</td><td valign='top'>:</td><td valign='top'><b>" & safeMessage & "</b></td></tr>"
            mailBody &= "</table>"

            mailBody &= "<br /><br />"

            mailBody &= "<span style='font-family: Cambria; font-size: 16px;'>Please check it on the online ordering portal and take the necessary action as soon as possible.</span>"
            mailBody &= "<br /><br />"
            mailBody &= "<span style='font-family: Cambria; font-size: 14px; font-style: italic;'>* Please use the <u>Ticket Code</u> as the keyword when searching on the Ticket page in BOOS.</span>"

            mailBody &= "<br /><br /><br />"

            mailBody &= "<span style='font-family: Cambria; font-size:16px;'>Kind Regards,</span>"
            mailBody &= "<br /><br /><br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px; font-weight: bold;'>Reza Andika Pratama</span><span style='font-family: Cambria; font-size:16px;'> | Developer</span>"
            mailBody &= "<br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px;'>reza@bigblinds.co.id</span>"
            mailBody &= "<br /><br /><br />"

            Dim myMail As New MailMessage()
            myMail.Subject = String.Format("[{0}] {1}", ticketCode, ticketSubject)
            myMail.From = New MailAddress(mailServer, mailAlias)

            If Not String.IsNullOrEmpty(mailTo) Then
                For Each thisMail In mailTo.Split(";"c)
                    If Not String.IsNullOrEmpty(thisMail.Trim()) Then myMail.To.Add(thisMail.Trim())
                Next
            End If

            If Not String.IsNullOrEmpty(mailCc) Then
                For Each thisMail In mailCc.Split(";"c)
                    If Not String.IsNullOrEmpty(thisMail.Trim()) Then myMail.CC.Add(thisMail.Trim())
                Next
            End If

            If Not String.IsNullOrEmpty(mailBcc) Then
                For Each thisMail In mailBcc.Split(";"c)
                    If Not String.IsNullOrEmpty(thisMail.Trim()) Then myMail.Bcc.Add(thisMail.Trim())
                Next
            End If

            myMail.Body = mailBody
            myMail.IsBodyHtml = True
            Dim smtpClient As New SmtpClient()
            smtpClient.Host = mailHost
            smtpClient.Port = mailPort
            smtpClient.EnableSsl = mailEnableSSL
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
            smtpClient.Timeout = 120000

            If mailNetworkCredentials Then
                smtpClient.UseDefaultCredentials = False
                smtpClient.Credentials = New NetworkCredential(mailAccount, mailPassword)
            Else
                smtpClient.UseDefaultCredentials = mailDefaultCredentials
            End If

            smtpClient.Send(myMail)
        Catch ex As Exception
        End Try
    End Sub

    Public Sub ResetPassword(userId As String, newPassword As String)
        If String.IsNullOrEmpty(userId) Then Exit Sub

        Dim thisData As DataRow = GetDataRow("SELECT CustomerLogins.*, Customers.CompanyId AS CompanyId FROM CustomerLogins LEFT JOIN Customers ON CustomerLogins.CustomerId=Customers.Id WHERE CustomerLogins.Id='" & userId & "'")
        If thisData Is Nothing Then Exit Sub

        Dim userName As String = thisData("UserName").ToString()
        Dim userEmail As String = thisData("Email").ToString()
        Dim fullName As String = thisData("FullName").ToString()
        Dim companyId As String = thisData("CompanyId").ToString()

        Dim mailData As DataRow = GetDataRow("SELECT * FROM Mailings WHERE CompanyId='" & companyId & "' AND Name='Reset Password' AND Active=1")
        If mailData Is Nothing Then Exit Sub

        Dim mailServer As String = mailData("Server").ToString()
        Dim mailHost As String = mailData("Host").ToString()
        Dim mailPort As Integer = mailData("Port")

        Dim mailAccount As String = mailData("Account").ToString()
        Dim mailPassword As String = mailData("Password").ToString()
        Dim mailAlias As String = mailData("Alias").ToString()
        Dim mailSubject As String = mailData("Subject").ToString()

        Dim mailTo As String = mailData("To").ToString()
        Dim mailCc As String = mailData("Cc").ToString()
        Dim mailBcc As String = mailData("Bcc").ToString()

        Dim mailNetworkCredentials As Boolean = mailData("NetworkCredentials")
        Dim mailDefaultCredentials As Boolean = mailData("DefaultCredentials")
        Dim mailEnableSSL As Boolean = mailData("EnableSSL")

        Dim mailBody As String = String.Empty

        mailBody = "<span style='font-family: Cambria; font-size: 16px;'>"
        mailBody &= "Hi <b>" & userName & "</b>,"
        mailBody &= "</span>"

        mailBody &= "<br /><br />"

        mailBody &= "<span style='font-family: Cambria; font-size: 16px;'>We have detected a new password request from the system.</span>"
        mailBody &= "<br /><br />"
        mailBody &= "<span style='font-family: Cambria; font-size: 16px;'>Below is the new password we have generated for you.</span>"
        mailBody &= "<br />"
        mailBody &= "<span style='font-family: Cambria; font-size: 16px;'>Please log in to the system, and you will be required to change your password upon your first login.</span>"

        mailBody &= "<br /><br />"
        mailBody &= "<span style='font-family: Cambria; font-size: 18px; font-weight: bold;'>User : " & userName & "</span>"
        mailBody &= "<br />"
        mailBody &= "<span style='font-family: Cambria; font-size: 18px; font-weight: bold;'>Password : " & newPassword & "</span>"

        mailBody &= "<br /><br /><br />"

        mailBody &= "<span style='font-family: Cambria; font-size: 16px;'>If you did not make this request, please disregard this message.</span>"
        mailBody &= "<br />"
        mailBody &= "<span style='font-family: Cambria; font-size: 16px;'>However, for security reasons, we recommend that you change your password as a precaution.</span>"

        mailBody &= "<br /><br /><br />"

        mailBody &= "<span style='font-family: Cambria; font-size:16px;'>Kind Regards,</span>"
        mailBody &= "<br /><br /><br />"
        mailBody &= "<span style='font-family: Cambria; font-size:16px; font-weight: bold;'>Reza Andika Pratama</span><span style='font-family: Cambria; font-size:16px;'> | Developer</span>"
        mailBody &= "<br />"
        mailBody &= "<span style='font-family: Cambria; font-size:16px;'>reza@bigblinds.co.id</span>"
        mailBody &= "<br /><br /><br />"

        Dim myMail As New MailMessage()
        myMail.Subject = mailSubject
        myMail.From = New MailAddress(mailServer, mailAlias)

        myMail.To.Add(userEmail)
        If Not String.IsNullOrEmpty(mailCc) Then
            For Each thisMail In mailCc.Split(";"c)
                If Not String.IsNullOrEmpty(thisMail.Trim()) Then myMail.CC.Add(thisMail.Trim())
            Next
        End If

        If Not String.IsNullOrEmpty(mailBcc) Then
            For Each thisMail In mailBcc.Split(";"c)
                If Not String.IsNullOrEmpty(thisMail.Trim()) Then myMail.Bcc.Add(thisMail.Trim())
            Next
        End If

        myMail.Body = mailBody
        myMail.IsBodyHtml = True

        Dim smtpClient As New SmtpClient()
        smtpClient.Host = mailHost
        smtpClient.Port = mailPort
        smtpClient.EnableSsl = mailEnableSSL
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
        smtpClient.Timeout = 120000

        If mailNetworkCredentials Then
            smtpClient.UseDefaultCredentials = False
            smtpClient.Credentials = New NetworkCredential(mailAccount, mailPassword)
        Else
            smtpClient.UseDefaultCredentials = mailDefaultCredentials
        End If

        smtpClient.Send(myMail)
    End Sub

    Public Sub Proforma3Days()
        Try

        Catch ex As Exception
        End Try
    End Sub

    Public Sub NewCustomer(customerId As String, loginId As String)
        Try
            If String.IsNullOrEmpty(customerId) OrElse String.IsNullOrEmpty(loginId) Then Exit Sub

            Dim customerData As DataRow = GetDataRow("SELECT Customers.*, CustomerLogins.FullName AS CustOperator, CASE WHEN Customers.CashSale=1 THEN 'Yes' WHEN Customers.CashSale=0 THEN 'No' ELSE 'Error' END AS CustomerCashSale FROM Customers LEFT JOIN CustomerLogins ON Customers.Operator=CustomerLogins.Id WHERE Customers.Id='" & customerId & "'")
            If customerData Is Nothing Then Exit Sub

            Dim debtorCode As String = customerData("DebtorCode").ToString()
            Dim customerName As String = customerData("Name").ToString()
            Dim companyId As String = customerData("CompanyId").ToString()
            Dim customerArea As String = customerData("Area").ToString()
            Dim customerOperator As String = customerData("Operator").ToString()
            Dim operatorName As String = customerData("CustOperator").ToString()
            Dim customerCashSale As String = customerData("CustomerCashSale").ToString()

            Dim priceGroupId As String = customerData("PriceGroupId").ToString()
            Dim shutterPriceGroupId As String = customerData("ShutterPriceGroupId").ToString()
            Dim doorPriceGroupId As String = customerData("DoorPriceGroupId").ToString()

            Dim priceGroupName As String = GetItemData("SELECT Name FROM PriceGroups WHERE Id='" & priceGroupId & "'")
            Dim shutterPriceGroupName As String = GetItemData("SELECT Name FROM PriceGroups WHERE Id='" & shutterPriceGroupId & "'")
            Dim doorPriceGroupName As String = GetItemData("SELECT Name FROM PriceGroups WHERE Id='" & doorPriceGroupId & "'")

            Dim companyName As String = GetItemData("SELECT Name FROM Companys WHERE Id='" & companyId & "'")
            Dim emailOperator As String = GetItemData("SELECT Email FROM CustomerLogins WHERE Id='" & customerOperator & "'")

            Dim mailData As DataRow = GetDataRow("SELECT * FROM Mailings WHERE CompanyId='" & companyId & "' AND Name='New Customer' AND Active=1")
            If mailData Is Nothing Then Exit Sub

            Dim mailServer As String = mailData("Server").ToString()
            Dim mailHost As String = mailData("Host").ToString()
            Dim mailPort As Integer = mailData("Port")

            Dim mailAccount As String = mailData("Account").ToString()
            Dim mailPassword As String = mailData("Password").ToString()
            Dim mailAlias As String = mailData("Alias").ToString()
            Dim mailSubject As String = mailData("Subject").ToString()

            Dim mailTo As String = mailData("To").ToString()
            Dim mailCc As String = mailData("Cc").ToString()
            Dim mailBcc As String = mailData("Bcc").ToString()

            Dim mailNetworkCredentials As Boolean = mailData("NetworkCredentials")
            Dim mailDefaultCredentials As Boolean = mailData("DefaultCredentials")
            Dim mailEnableSSL As Boolean = mailData("EnableSSL")

            Dim fullName As String = GetItemData("SELECT FullName FROM CustomerLogins WHERE Id='" & loginId & "'")

            Dim mailBody As String = String.Empty

            mailBody = "<span style='font-family: Cambria; font-size: 16px;'>"
            mailBody &= "Hi Bella & Team,"
            mailBody &= "<br /><br />"
            mailBody &= "A new customer has been added to the online ordering portal by "
            mailBody &= "<b>" & fullName & "</b>, with the following details:"
            mailBody &= "</span>"

            mailBody &= "<br /><br />"

            mailBody &= "<table cellpadding='3' cellspacing='0' style='font-family:Cambria; font-size: 16px;'>"
            mailBody &= "<tr><td valign='top'>Company</td><td valign='top'>:</td><td valign='top'><b>" & companyName & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Debtor Code</td><td valign='top'>:</td><td valign='top'><b>" & debtorCode & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Customer Name</td><td valign='top'>:</td><td valign='top'><b>" & customerName & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Operator</td><td valign='top'>:</td><td valign='top'><b>" & operatorName & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Area</td><td valign='top'>:</td><td valign='top'><b>" & customerArea & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Cash Sale</td><td valign='top'>:</td><td valign='top'><b>" & customerCashSale & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'></td><td valign='top'></td><td valign='top'></td></tr>"
            mailBody &= "<tr><td valign='top'></td><td valign='top'></td><td valign='top'></td></tr>"
            mailBody &= "<tr><td valign='top'></td><td valign='top'></td><td valign='top'></td></tr>"
            mailBody &= "<tr><td valign='top'>Blinds Pricing</td><td valign='top'>:</td><td valign='top'><b>" & priceGroupName & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Shutters Pricing</td><td valign='top'>:</td><td valign='top'><b>" & shutterPriceGroupName & "</b></td></tr>"
            mailBody &= "<tr><td valign='top'>Doors Pricing</td><td valign='top'>:</td><td valign='top'><b>" & doorPriceGroupName & "</b></td></tr>"

            mailBody &= "</table>"

            mailBody &= "<br /><br /><br />"

            mailBody &= "<span style='font-family: Cambria; font-size: 16px;'>Please check and proceed with the data integration.</span>"

            mailBody &= "<br /><br /><br />"

            mailBody &= "<span style='font-family: Cambria; font-size:16px;'>Kind Regards,</span>"
            mailBody &= "<br /><br /><br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px;font-weight: bold;'>Reza Andika Pratama</span><span style='font-family: Cambria; font-size:16px;'> | Developer</span>"
            mailBody &= "<br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px;'>reza@bigblinds.co.id</span>"

            Dim myMail As New MailMessage()
            myMail.Subject = String.Format("New Customer | {0}", customerName)
            myMail.From = New MailAddress(mailServer, mailAlias)

            If Not String.IsNullOrEmpty(mailTo) Then
                For Each thisMail In mailTo.Split(";"c)
                    If Not String.IsNullOrEmpty(thisMail.Trim()) Then myMail.To.Add(thisMail.Trim())
                Next
            End If

            If Not String.IsNullOrEmpty(mailCc) Then
                For Each thisMail In mailCc.Split(";"c)
                    If Not String.IsNullOrEmpty(thisMail.Trim()) Then myMail.CC.Add(thisMail.Trim())
                Next
            End If

            If Not String.IsNullOrEmpty(emailOperator) Then
                myMail.CC.Add(emailOperator)
            End If

            If Not String.IsNullOrEmpty(mailBcc) Then
                For Each thisMail In mailBcc.Split(";"c)
                    If Not String.IsNullOrEmpty(thisMail.Trim()) Then myMail.Bcc.Add(thisMail.Trim())
                Next
            End If

            myMail.Body = mailBody
            myMail.IsBodyHtml = True
            Dim smtpClient As New SmtpClient()
            smtpClient.Host = mailHost
            smtpClient.Port = mailPort
            smtpClient.EnableSsl = mailEnableSSL
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
            smtpClient.Timeout = 120000

            If mailNetworkCredentials Then
                smtpClient.UseDefaultCredentials = False
                smtpClient.Credentials = New NetworkCredential(mailAccount, mailPassword)
            Else
                smtpClient.UseDefaultCredentials = mailDefaultCredentials
            End If

            smtpClient.Send(myMail)
        Catch ex As Exception
        End Try
    End Sub

    Public Sub LoginCredentials(customerId As String, loginId As String, action As String)
        Try
            Dim settingClass As New SettingClass

            If String.IsNullOrEmpty(customerId) OrElse String.IsNullOrEmpty(loginId) Then Exit Sub

            Dim customerData As DataRow = GetDataRow("SELECT Customers.*, Companys.Name AS CompanyName, CustomerLogins.FullName AS OperatorName, CustomerLogins.Email AS OperatorEmail FROM Customers LEFT JOIN Companys ON Customers.CompanyId=Companys.Id LEFT JOIN CustomerLogins ON Customers.Operator=CustomerLogins.Id WHERE Customers.Id='" & customerId & "'")
            If customerData Is Nothing Then Exit Sub
            Dim companyId As String = customerData("CompanyId").ToString()
            Dim customerName As String = customerData("nAME").ToString()
            Dim companyName As String = customerData("CompanyName").ToString()
            Dim operatorName As String = customerData("OperatorName").ToString()
            Dim operatorEmail As String = customerData("OperatorEmail").ToString()

            Dim contactData As DataRow = GetDataRow("SELECT * FROM CustomerContacts WHERE CustomerId='" & customerId & "' AND [Primary]=1")
            If contactData Is Nothing Then Exit Sub
            Dim contactName As String = contactData("Name").ToString()
            Dim contactEmail As String = contactData("Email").ToString()

            Dim loginData As DataTable = GetDataTable("SELECT * FROM CustomerLogins WHERE CustomerId='" & customerId & "' AND Active=1 ORDER BY UserName ASC")
            If loginData.Rows.Count = 0 Then Exit Sub

            Dim actionData As DataRow = GetDataRow("SELECT CustomerLogins.*, LoginRoles.Name AS RoleName FROM CustomerLogins LEFT JOIN LoginRoles ON CustomerLogins.RoleId=LoginRoles.Id WHERE CustomerLogins.Id='" & loginId & "'")
            If actionData Is Nothing Then Exit Sub
            Dim actionName As String = actionData("FullName").ToString()
            Dim actionRole As String = actionData("RoleName").ToString()
            Dim actionEmail As String = actionData("Email").ToString()

            Dim mailData As DataRow = GetDataRow("SELECT * FROM Mailings WHERE CompanyId='" & companyId & "' AND Name='" & action & "' AND Active=1")
            If mailData Is Nothing Then Exit Sub

            Dim mailServer As String = mailData("Server").ToString()
            Dim mailHost As String = mailData("Host").ToString()
            Dim mailPort As Integer = mailData("Port")

            Dim mailAccount As String = mailData("Account").ToString()
            Dim mailPassword As String = mailData("Password").ToString()
            Dim mailAlias As String = mailData("Alias").ToString()
            Dim mailSubject As String = mailData("Subject").ToString()

            Dim mailTo As String = mailData("To").ToString()
            Dim mailCc As String = mailData("Cc").ToString()
            Dim mailBcc As String = mailData("Bcc").ToString()

            Dim mailNetworkCredentials As Boolean = mailData("NetworkCredentials")
            Dim mailDefaultCredentials As Boolean = mailData("DefaultCredentials")
            Dim mailEnableSSL As Boolean = mailData("EnableSSL")

            Dim mailBody As String = String.Empty

            mailBody = "<span style='font-family: Cambria; font-size: 16px;'>"
            mailBody &= "Hi <b>" & contactName & "</b>,"
            mailBody &= "<br /><br />"

            If action = "Welcome Customer" Then
                mailBody &= "Welcome and thank you for choosing us! 🎉"
                mailBody &= "<br />"
                mailBody &= "We’re excited to have you as our new customer."
                mailBody &= "<br /><br />"
                mailBody &= "Below are your details and login access to our system:"
                mailBody &= "<br /><br />"
            End If

            If action = "Login Credentials" Then
                mailBody &= "This email is to re-share your login access to our system and important information regarding our order process."
                mailBody &= "<br /><br />"
            End If
            mailBody &= "</span>"

            mailBody &= "<span style='font-family: Cambria; font-size: 16px;'>"
            mailBody &= "<b>Customer Information</b>"
            mailBody &= "<br />"
            mailBody &= "</span>"
            mailBody &= "<table cellpadding='3' cellspacing='0' style='font-family:Cambria; font-size: 16px;'>"
            mailBody &= "<tr><td valign='top'>Name</td><td valign='top'>:</td><td valign='top'>" & customerName & "</td></tr>"
            mailBody &= "<tr><td valign='top'>Company</td><td valign='top'>:</td><td valign='top'>" & companyName & "</td></tr>"
            mailBody &= "</table>"
            mailBody &= "<br />"

            mailBody &= "<span style='font-family: Cambria; font-size: 16px;'>"
            mailBody &= "<b>Login Details</b>"
            mailBody &= "</span>"
            mailBody &= "<table style='border-collapse:collapse; width:60%; font-family:Cambria; font-size:16px;'>"
            mailBody &= "<tr style='background-color:#00B0F0; color:black; font-weight:bold; text-align:center;'>"
            mailBody &= "<th style='border:1px solid #000; padding:6px;'>No</th>"
            mailBody &= "<th style='border:1px solid #000; padding:6px;'>User</th>"
            mailBody &= "<th style='border:1px solid #000; padding:6px;'>Temporary Password</th>"
            mailBody &= "</tr>"

            Dim no As Integer = 1
            For i As Integer = 0 To loginData.Rows.Count - 1
                mailBody &= "<tr>"
                mailBody &= "<td style='border:1px solid #000; padding:6px; text-align:center;'>" & no & "</td>"
                mailBody &= "<td style='border:1px solid #000; padding:6px;'>" & loginData.Rows(i)("UserName").ToString() & "</td>"
                mailBody &= "<td style='border:1px solid #000; padding:6px;'>" & settingClass.Decrypt(loginData.Rows(i)("Password").ToString()) & "</td>"
                mailBody &= "</tr>"
                no += 1
            Next
            mailBody &= "</table>"

            mailBody &= "<span style='font-family: Cambria; font-size: 16px;'>"
            If companyId = "2" Then
                mailBody &= "<br /><br />"
                If action = "Welcome Customer" Then
                    mailBody &= "For security reasons, you will be required to change your password after your first login."
                    mailBody &= "<br /><br />"
                End If
                If action = "Login Credentials" Then
                    mailBody &= "For security reasons, please change your password after receiving this email."
                    mailBody &= "<br /><br />"
                End If

                mailBody &= "<b>Who to Contact (Based on Your Needs):</b>"
                mailBody &= "<br /><br />"

                mailBody &= "<b>Sales / Orders:</b>"
                mailBody &= "<ul>"
                mailBody &= "<li>Matt McCamey - matt@jpmdirect.com.au</li>"
                If Not String.IsNullOrEmpty(operatorName) Then
                    mailBody &= "<li>" & operatorName & " - " & operatorEmail & "</li>"
                End If
                mailBody &= "</ul>"
                mailBody &= "<br />"

                mailBody &= "<b>Customer Service / Order Status & Production Enquiries:</b>"
                mailBody &= "<ul><li>Indra - order@jpmdirect.com.au</li></ul>"
                mailBody &= "<br />"

                mailBody &= "<b>Finance / Billing:</b>"
                mailBody &= "<ul>"
                mailBody &= "<li>Bella - invoice@jpmdirect.com.au</li>"
                mailBody &= "<li>Fariz - fariz@jpmdirect.com.au</li>"
                mailBody &= "<li>Rosye - accounts@jpmdirect.com.au</li>"
                mailBody &= "</ul>"

                mailBody &= "<br />"
            End If

            mailBody &= "Order Process & Important Notes:"
            mailBody &= "<br /><br />"
            mailBody &= "<ul>"
            mailBody &= "<li>We will check and confirm each order within <b>24 hours</b>.</li>"
            mailBody &= "<li>If there are any questions regarding order details, we will contact you. Otherwise, the order will be put into production and shipped on the earliest available container.</li>"
            mailBody &= "<li>Once you have received and confirmed the order confirmation, the order <b>cannot be cancelled</b>.</li>"
            mailBody &= "<li>When placing future orders, our factory will make all deductions. You only need to provide <b>pin-to-pin</b> or <b>bracket-to-bracket</b> measurements.</li>"
            mailBody &= "<li>If you have any questions about this procedure, please do not hesitate to ask.</li>"
            mailBody &= "</ul>"
            mailBody &= "<br /><br />"

            If action = "Welcome Customer" Then
                mailBody &= "We look forward to a long and prosperous relationship."
            End If
            If action = "Login Credentials" Then
                mailBody &= "If you have already logged in and updated your password, you may ignore the login details above."
            End If
            mailBody &= "</span>"

            mailBody &= "<br /><br /><br />"

            mailBody &= "<span style='font-family: Cambria; font-size:16px;'>Kind Regards,</span>"
            mailBody &= "<br /><br /><br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px;font-weight: bold;'>" & actionName & "</span>"
            mailBody &= "<br />"
            mailBody &= "<span style='font-family: Cambria; font-size:16px'>" & actionRole & "</span>"

            Dim myMail As New MailMessage()
            myMail.Subject = mailSubject
            myMail.From = New MailAddress(mailServer, mailAlias)

            myMail.To.Add(contactEmail)
            For Each thisMail In mailCc.Split(";"c)
                If Not String.IsNullOrEmpty(thisMail.Trim()) Then myMail.CC.Add(thisMail.Trim())
            Next
            If Not String.IsNullOrEmpty(operatorEmail) Then
                myMail.CC.Add(operatorEmail)
            End If
            If Not String.IsNullOrEmpty(mailBcc) Then
                For Each thisMail In mailBcc.Split(";"c)
                    If Not String.IsNullOrEmpty(thisMail.Trim()) Then myMail.Bcc.Add(thisMail.Trim())
                Next
            End If

            myMail.Body = mailBody
            myMail.IsBodyHtml = True
            Dim smtpClient As New SmtpClient()
            smtpClient.Host = mailHost
            smtpClient.Port = mailPort
            smtpClient.EnableSsl = mailEnableSSL
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network
            smtpClient.Timeout = 120000

            If mailNetworkCredentials Then
                smtpClient.UseDefaultCredentials = False
                smtpClient.Credentials = New NetworkCredential(mailAccount, mailPassword)
            Else
                smtpClient.UseDefaultCredentials = mailDefaultCredentials
            End If

            smtpClient.Send(myMail)
        Catch ex As Exception
        End Try
    End Sub
End Class
