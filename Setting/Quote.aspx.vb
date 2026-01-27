Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.Drawing.Drawing2D

Partial Class Setting_Quote
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim customerId As String = String.Empty
    Dim url As String = String.Empty

    Dim settingClass As New SettingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("accountid")) Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        customerId = Request.QueryString("accountid").ToString()
        If customerId <> Session("CustomerId") Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(customerId)
        End If
    End Sub

    Protected Sub linkLogo_Click(sender As Object, e As EventArgs)
        MessageError_Logo(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showLogo(); };"
        Try

            ClientScript.RegisterStartupScript(Me.GetType(), "showLogo", thisScript, True)
        Catch ex As Exception
            MessageError_Logo(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Logo(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showLogo", thisScript, True)
        End Try
    End Sub

    Protected Sub linkAddress_Click(sender As Object, e As EventArgs)
        MessageError_Address(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showAddress(); };"
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerQuotes WHERE Id='" & customerId & "'")
            If myData Is Nothing Then Exit Sub

            txtAddress.Text = myData("Address").ToString
            txtSuburb.Text = myData("Suburb").ToString
            txtState.Text = myData("State").ToString
            txtPostCode.Text = myData("PostCode").ToString
            ddlCountry.SelectedValue = myData("Country").ToString

            ClientScript.RegisterStartupScript(Me.GetType(), "showAddress", thisScript, True)
        Catch ex As Exception
            MessageError_Address(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Address(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showAddress", thisScript, True)
        End Try
    End Sub

    Protected Sub linkContact_Click(sender As Object, e As EventArgs)
        MessageError_Contact(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showContact(); };"
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerQuotes WHERE Id='" & customerId & "'")
            If myData Is Nothing Then Exit Sub

            txtEmail.Text = myData("Email").ToString
            txtPhone.Text = myData("Phone").ToString

            ClientScript.RegisterStartupScript(Me.GetType(), "showContact", thisScript, True)
        Catch ex As Exception
            MessageError_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Contact(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showContact", thisScript, True)
        End Try
    End Sub

    Protected Sub linkTerms_Click(sender As Object, e As EventArgs)
        MessageError_Terms(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showTerms(); };"
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerQuotes WHERE Id='" & customerId & "'")
            If myData Is Nothing Then Exit Sub

            Dim termText As String = myData("Terms").ToString()
            termText = termText.Replace(vbCrLf, "<br>").Replace(vbLf, "<br>")
            txtTerms.Text = myData("Terms").ToString()

            ClientScript.RegisterStartupScript(Me.GetType(), "showTerms", thisScript, True)
        Catch ex As Exception
            MessageError_Terms(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Terms(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showTerms", thisScript, True)
        End Try
    End Sub

    Protected Sub btnLogo_Click(sender As Object, e As EventArgs)
        MessageError_Logo(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showLogo(); };"
        Try
            If Not fuLogo.HasFiles Then
                MessageError_Logo(True, "Invalid file type. Only JPG, PNG, GIF are allowed.")
                ClientScript.RegisterStartupScript(Me.GetType(), "showLogo", thisScript, True)
                Exit Sub
            End If

            Dim newLogo As String = String.Empty

            Dim allowedExtensions As String() = {".jpg", ".jpeg", ".png", ".gif"}
            Dim ext As String = IO.Path.GetExtension(fuLogo.FileName.ToString()).ToLower()

            If Not allowedExtensions.Contains(ext) Then
                MessageError_Logo(True, "Invalid file type. Only JPG, PNG, GIF are allowed.")
                ClientScript.RegisterStartupScript(Me.GetType(), "showLogo", thisScript, True)
                Exit Sub
            End If

            Dim width As Integer = 440
            Dim height As Integer = 120

            Dim inp_Stream As IO.Stream = fuLogo.PostedFile.InputStream

            Using image = Drawing.Image.FromStream(inp_Stream)
                Dim ratio As Double = Math.Min(CDbl(width) / image.Width, CDbl(height) / image.Height)
                Dim newWidth As Integer = CInt(image.Width * ratio)
                Dim newHeight As Integer = CInt(image.Height * ratio)

                Dim myImg As New Bitmap(newWidth, newHeight)
                Dim myImgGraph As Graphics = Graphics.FromImage(myImg)
                myImgGraph.CompositingQuality = CompositingQuality.HighQuality
                myImgGraph.SmoothingMode = SmoothingMode.HighQuality
                myImgGraph.InterpolationMode = InterpolationMode.HighQualityBicubic

                If Not String.IsNullOrEmpty(lblOldLogo.Text) AndAlso Not {"accent.png", "jpmdirect.jpg", "yourlogo.png"}.Contains(lblOldLogo.Text) Then
                    Dim oldFilePath As String = Server.MapPath(String.Format("~/assets/images/logo/customers/{0}", lblOldLogo.Text))
                    If IO.File.Exists(oldFilePath) Then
                        IO.File.Delete(oldFilePath)
                    End If
                End If

                Dim imgRectangle = New Rectangle(0, 0, newWidth, newHeight)
                myImgGraph.DrawImage(image, imgRectangle)

                newLogo = String.Format("{0}{1}{2}", Now.ToString("ddMMyyyyHHmmss"), customerId, ext)
                Dim path = IO.Path.Combine(Server.MapPath("~/assets/images/logo/customers"), newLogo)

                myImg.Save(path, image.RawFormat)
            End Using

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerQuotes SET Logo=@Logo WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", customerId)
                    myCmd.Parameters.AddWithValue("@Logo", newLogo)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            url = String.Format("~/setting/quote?accountid={0}", customerId)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Logo(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Logo(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showLogo", thisScript, True)
        End Try
    End Sub

    Protected Sub btnAddress_Click(sender As Object, e As EventArgs)
        MessageError_Address(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showAddress(); };"
        Try
            If msgErrorAddress.InnerText = "" Then
                If Session("CompanyId") = "2" Then ddlCountry.SelectedValue = "Australia"
                If Session("CompanyId") = "3" OrElse Session("CompanyId") = "5" Then ddlCountry.SelectedValue = "Indonesia"

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerQuotes SET Address=@Address, Suburb=@Suburb, State=@State, PostCode=@PostCode, Country=@Country WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", customerId)
                        myCmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Suburb", txtSuburb.Text.Trim())
                        myCmd.Parameters.AddWithValue("@State", txtState.Text.Trim())
                        myCmd.Parameters.AddWithValue("@PostCode", txtPostCode.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Country", ddlCountry.SelectedValue)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                url = String.Format("~/setting/quote?accountid={0}", customerId)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_Address(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Address(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showAddress", thisScript, True)
        End Try
    End Sub

    Protected Sub btnContact_Click(sender As Object, e As EventArgs)
        MessageError_Contact(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showContact(); };"
        Try
            If msgErrorContact.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerQuotes SET Email=@Email, Phone=@Phone WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", customerId)
                        myCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim())

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                url = String.Format("~/setting/quote?accountid={0}", customerId)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Contact(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showContact", thisScript, True)
        End Try
    End Sub

    Protected Sub btnTerms_Click(sender As Object, e As EventArgs)
        MessageError_Terms(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showTerms(); };"
        Try
            If msgErrorTerms.InnerText = "" Then
                Dim terms As String = txtTerms.Text.Trim()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerQuotes SET Terms=@Terms WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", customerId)
                        myCmd.Parameters.AddWithValue("@Terms", txtTerms.Text)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                url = String.Format("~/setting/quote?accountid={0}", customerId)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_Terms(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Terms(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showTerms", thisScript, True)
        End Try
    End Sub

    Protected Sub BindData(customerId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerQuotes WHERE Id='" & customerId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/", False)
                Exit Sub
            End If

            imgQuote.ImageUrl = String.Format("~/assets/images/logo/customers/{0}", thisData("Logo").ToString())
            lblOldLogo.Text = thisData("Logo").ToString()

            Dim companyId As String = settingClass.GetItemData("SELECT CompanyId FROM Customers WHERE Id='" & customerId & "'")

            Dim address As String = String.Empty

            address &= String.Format("- Address : {0}", thisData("Address").ToString())
            address &= "<br />"
            address &= String.Format("- Suburb : {0}", thisData("Suburb").ToString())
            address &= "<br />"
            address &= String.Format("- State : {0}", thisData("State").ToString())
            address &= "<br />"
            address &= String.Format("- Post Code : {0}", thisData("PostCode").ToString())
            address &= "<br />"
            address &= String.Format("- Country : {0}", thisData("Country").ToString())

            Dim email As String = thisData("Email").ToString()
            Dim phone As String = thisData("Phone").ToString()

            Dim contact As String = String.Empty
            If Not String.IsNullOrEmpty(email) Then
                contact &= String.Format("- Email : {0}", email)
                contact &= "<br />"
            End If
            If Not String.IsNullOrEmpty(phone) Then
                contact &= String.Format("- Phone : {0}", phone)
            End If

            Dim termText As String = thisData("Terms").ToString()
            termText = termText.Replace(vbCrLf, "<br>").Replace(vbLf, "<br>")

            pAddress.InnerHtml = address
            pContact.InnerHtml = contact
            pTerms.InnerHtml = termText

            imgQuoteExample.ImageUrl = "~/assets/images/quotation/full.jpg"
            imgQuoteExample2.ImageUrl = "~/assets/images/quotation/full.jpg"
            If companyId = "1" OrElse companyId = "3" Then
                imgQuoteExample.ImageUrl = "~/assets/images/quotation/local.jpg"
                imgQuoteExample2.ImageUrl = "~/assets/images/quotation/local.jpg"
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Logo(visible As Boolean, message As String)
        divErrorLogo.Visible = visible : msgErrorLogo.InnerText = message
    End Sub

    Protected Sub MessageError_Address(visible As Boolean, message As String)
        divErrorAddress.Visible = visible : msgErrorAddress.InnerText = message
    End Sub

    Protected Sub MessageError_Contact(visible As Boolean, message As String)
        divErrorContact.Visible = visible : msgErrorContact.InnerText = message
    End Sub

    Protected Sub MessageError_Terms(visible As Boolean, message As String)
        divErrorTerms.Visible = visible : msgErrorTerms.InnerText = message
    End Sub

    Protected Function PageAction(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
