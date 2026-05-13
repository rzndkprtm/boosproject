Imports System.Data
Imports System.Data.SqlClient

Partial Class Order_Unshipment
    Inherits Page

    Dim unshipmentClass As New OrderClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            MessageError_Email(False, String.Empty)
            BindCompany()
            BindDataOrder(txtSearch.Text, ddlCompany.SelectedValue)

            btnPreview.OnClientClick = "window.open('view?action=unshipment&search=" & Server.UrlEncode(txtSearch.Text.Trim()) & "&company=" & Server.UrlEncode(ddlCompany.SelectedValue) & "','_blank'); return false;"
        End If
    End Sub

    Protected Sub btnDownload_Click(sender As Object, e As EventArgs)
        Try
            Dim unshipmentClass As New UnshipmentClass
            Dim pdfBytes As Byte() = unshipmentClass.BindContent(txtSearch.Text, ddlCompany.SelectedValue, Session("RoleName").ToString(), Session("LevelName").ToString(), Session("LoginId"))

            Dim fileName As String = String.Format("ORDER {0} {1}.pdf", String.Empty, String.Empty)

            Response.Clear()
            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "attachment; filename=UNSHIPMENT ORDER.pdf")
            Response.BinaryWrite(pdfBytes)
            Response.Flush()
            Response.End()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnSendEmail_Click(sender As Object, e As EventArgs)
        MessageError_Email(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showSendEmail(); };"
        Try
            If String.IsNullOrEmpty(txtSendEmailTo.Text) Then
                MessageError_Email(True, "EMAIL TO IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showSendEmail", thisScript, True)
                Exit Sub
            End If

            Dim toEmail As String = String.Empty
            If Not String.IsNullOrEmpty(txtSendEmailTo.Text) Then
                Dim raw As String = txtSendEmailTo.Text
                Dim lines As String() = raw.Split(New String() {vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)
                Dim cleanedEmails As New List(Of String)
                For Each line As String In lines
                    Dim email As String = line.Trim()
                    If email <> "" Then cleanedEmails.Add(email)
                Next
                toEmail = String.Join(";", cleanedEmails)
            End If

            Dim ccEmail As String = String.Empty
            If Not String.IsNullOrEmpty(txtSendEmailCC.Text) Then
                Dim raw As String = txtSendEmailCC.Text
                Dim lines As String() = raw.Split(New String() {vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)
                Dim cleanedEmails As New List(Of String)
                For Each line As String In lines
                    Dim email As String = line.Trim()
                    If email <> "" Then cleanedEmails.Add(email)
                Next
                ccEmail = String.Join(";", cleanedEmails)
            End If

            If msgErrorSendEmail.InnerText = "" Then
                Dim mailingClass As New MailingClass
                mailingClass.SentUnshipment(toEmail, ccEmail, txtSearch.Text.Trim(), ddlCompany.SelectedValue, Session("RoleName").ToString(), Session("LevelName").ToString(), Session("LoginId"))
            End If
        Catch ex As Exception
            MessageError_Email(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Email(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showSendEmail", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlCompany.SelectedValue)
    End Sub

    Protected Sub ddlCompany_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataOrder(txtSearch.Text, ddlCompany.SelectedValue)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindDataOrder(txtSearch.Text, ddlCompany.SelectedValue)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    url = String.Format("~/order/detail?orderid={0}", dataId)
                    Response.Redirect(url, False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError(True, "PLEASE CONTACT IT AT SUPPORT REZA@BIGBLINDS.CO.ID !")
                        If Session("RoleName") = "Customer" Then
                            MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                        End If
                    End If
                End Try
            End If
        End If
    End Sub

    Protected Sub BindDataOrder(searchText As String, companyId As String)
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(companyId), CType(DBNull.Value, Object), companyId)),
                New SqlParameter("@SearchText", searchText.Trim()),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@LoginId", Session("LoginId"))
            }

            Dim thisData As DataTable = unshipmentClass.GetDataTableSP("sp_OrderUnshipment", params)
            gvList.DataSource = thisData
            gvList.DataBind()

            gvList.Columns(1).Visible = PageAction("Visible ID")

            aEmail.Visible = PageAction("Email")

            divCompany.Visible = PageAction("Sort Company")

            Dim mailingString As String = "SELECT * FROM Mailings WHERE Name='Unshipment Order' AND Active=1"
            Dim dataToMailing As DataTable = unshipmentClass.GetDataTable(mailingString)
            If dataToMailing.Rows.Count > 0 Then
                Dim listTo As New List(Of String)
                Dim listCc As New List(Of String)

                For Each row As DataRow In dataToMailing.Rows
                    Dim emailsTo As String() = row("To").ToString().Split(";"c)
                    Dim emailsCc As String() = row("Cc").ToString().Split(";"c)

                    For Each email As String In emailsTo
                        If Not String.IsNullOrWhiteSpace(email) Then
                            listTo.Add(email.Trim())
                        End If
                    Next

                    For Each email As String In emailsCc
                        If Not String.IsNullOrWhiteSpace(email) Then
                            listCc.Add(email.Trim())
                        End If
                    Next
                Next

                txtSendEmailTo.Text = String.Join(vbCrLf, listTo)
                txtSendEmailCC.Text = String.Join(vbCrLf, listCc)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
            End If
        End Try
    End Sub

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            ddlCompany.DataSource = unshipmentClass.GetDataTable("SELECT * FROM Companys WHERE Active=1 ORDER BY Name ASC")
            ddlCompany.DataTextField = "Alias"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            ddlCompany.Items.Insert(0, New ListItem("All", ""))

        Catch ex As Exception
            ddlCompany.Items.Clear()
            ddlCompany.Items.Add(New ListItem("All", ""))
        End Try
    End Sub

    Protected Function BindFactory(headerId As Integer) As String
        Try
            Dim factoryList As New List(Of String)

            Dim detailData As DataTable = unshipmentClass.GetDataTable("SELECT OrderDetails.*, Products.Name AS ProductName, Designs.Name AS DesignName, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.DesignId=Blinds.Id WHERE OrderDetails.HeaderId='" & headerId & "'")
            For iDetail As Integer = 0 To detailData.Rows.Count - 1
                Dim designName As String = detailData.Rows(iDetail)("DesignName").ToString()
                Dim blindName As String = detailData.Rows(iDetail)("BlindName").ToString()

                Dim isBig As Boolean = False
                Dim isAustralia As Boolean = False
                Dim isTaiwan As Boolean = False
                Dim isChina As Boolean = False

                If designName = "Aluminium Blind" OrElse designName = "Design Shades" OrElse designName = "Linea Valance" OrElse designName = "Panel Glide" OrElse designName = "Pelmet" OrElse designName = "Roman Blind" OrElse designName = "Privacy Venetian" OrElse designName = "Venetian Blind" OrElse designName = "Vertical" OrElse designName = "Roller Blind" OrElse designName = "Sample" OrElse designName = "Skyline Shutter Express" OrElse designName = "Outdoor" OrElse designName = "Saphora Drape" OrElse designName = "Roller Horizon" Then
                    isBig = True
                End If

                If designName = "Cellular Shades" Then
                    Dim fabricColourId As String = detailData.Rows(iDetail)("FabricColourId").ToString()
                    Dim fabricFactory As String = unshipmentClass.GetItemData("SELECT Factory FROM FabricColours WHERE Id='" & fabricColourId & "'")

                    If fabricFactory = "Express" Then isBig = True
                    If fabricFactory = "Regular" Then isTaiwan = True
                End If

                If designName = "Curtain" Then
                    Dim fabricColourId As String = detailData.Rows(iDetail)("FabricColourId").ToString()
                    Dim fabricColourIdB As String = detailData.Rows(iDetail)("FabricColourIdB").ToString()

                    Dim trackType As String = detailData.Rows(iDetail)("TrackType").ToString()
                    Dim trackTypeB As String = detailData.Rows(iDetail)("TrackTypeB").ToString()

                    Dim fabricFactory As String = String.Empty
                    Dim fabricFactoryB As String = String.Empty

                    If fabricColourId <> "" Then
                        fabricFactory = unshipmentClass.GetItemData("SELECT Factory FROM FabricColours WHERE Id='" & fabricColourId & "'")
                    End If

                    If fabricColourIdB <> "" Then
                        fabricFactoryB = unshipmentClass.GetItemData("SELECT Factory FROM FabricColours WHERE Id='" & fabricColourIdB & "'")
                    End If

                    If fabricFactory = "Express" Then
                        isBig = True
                    ElseIf fabricFactory = "Regular" Then
                        isAustralia = True
                    End If

                    If fabricFactoryB = "Express" Then
                        isBig = True
                    ElseIf fabricFactoryB = "Regular" Then
                        isAustralia = True
                    End If

                    If trackType = "Standard Track" Then
                        isAustralia = True
                    ElseIf trackType = "Motorised Track" Then
                        isBig = True
                    End If

                    If trackTypeB = "Standard Track" Then
                        isAustralia = True
                    ElseIf trackTypeB = "Motorised Track" Then
                        isBig = True
                    End If
                End If

                If designName = "Skyline Shutter Ocean" OrElse designName = "Evolve Shutter Ocean" Then
                    isChina = True
                End If

                If designName = "Door" OrElse designName = "Window" Then
                    Dim frameColour As String = detailData.Rows(iDetail)("FrameColour").ToString()

                    If frameColour.Contains("Regular") Then isAustralia = True
                    If frameColour.Contains("Express") Then isBig = True
                End If

                If isBig AndAlso Not factoryList.Contains("BIG") Then
                    factoryList.Add("BIG")
                End If

                If isTaiwan AndAlso Not factoryList.Contains("TAIWAN") Then
                    factoryList.Add("TAIWAN")
                End If

                If isAustralia AndAlso Not factoryList.Contains("AUS") Then
                    factoryList.Add("AUS")
                End If

                If isChina AndAlso Not factoryList.Contains("CHINA") Then
                    factoryList.Add("CHINA")
                End If
            Next

            Dim factory As String = String.Join(", ", factoryList)

            Return factory
        Catch ex As Exception
            Return "ERROR"
        End Try
    End Function

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Email(visible As Boolean, message As String)
        divErrorSendEmail.Visible = visible : msgErrorSendEmail.InnerText = message
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
