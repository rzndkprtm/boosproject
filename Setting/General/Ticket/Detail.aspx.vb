Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Partial Class Setting_General_Ticket_Detail
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/general/ticket", False)
            Exit Sub
        End If
        If String.IsNullOrEmpty(Request.QueryString("ticketid")) Then
            Response.Redirect("~/setting/general/ticket", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("ticketid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = lblId.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM Tickets WHERE Id=@Id; DELETE FROM TicketMessages WHERE TicketId=@Id; DELETE FROM TicketFiles WHERE TicketId=@Id;", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim folderPath As String = Server.MapPath("~/File/Ticket/" & lblTicketNo.Text)
            If Directory.Exists(folderPath) Then
                Directory.Delete(folderPath, True)
            End If
            Response.Redirect("~/setting/general/ticket", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnDeleteDetail_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = lblId.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM Tickets WHERE Id=@Id; DELETE FROM TicketMessages WHERE TicketId=@Id; DELETE FROM TicketFiles WHERE TicketId=@Id;", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim folderPath As String = Server.MapPath("~/File/Ticket/" & lblTicketNo.Text)
            If Directory.Exists(folderPath) Then
                Directory.Delete(folderPath, True)
            End If
            Response.Redirect("~/setting/general/ticket", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(ticketId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT Tickets.*, LoginCreated.UserName AS CreatedUserName, LoginCreated.FullName AS CreatedFullName, LoginClosed.UserName AS ClosedUserName, LoginClosed.FullName AS ClosedFullName, Customers.Name AS CustomerName FROM dbo.Tickets INNER JOIN dbo.Logins LoginCreated ON Tickets.LoginId = LoginCreated.Id LEFT JOIN dbo.Logins LoginClosed ON Tickets.ClosedBy = LoginClosed.Id LEFT JOIN dbo.Customers ON LoginCreated.CustomerId = Customers.Id WHERE Tickets.Id='" & ticketId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/general/ticket", False)
                Exit Sub
            End If

            lblId.Visible = LoginAccess("Visible ID")
            lblTicketNo.Text = thisData("TicketNo").ToString()
            lblType.Text = thisData("Type").ToString()
            lblCustomerName.Text = thisData("CustomerName").ToString()
            lblCreatedFullName.Text = thisData("CreatedFullName").ToString()
            lblSubject.Text = thisData("Subject").ToString()

            aDelete.Visible = LoginAccess("Delete")

            gvList.DataSource = settingClass.GetDataTable("SELECT CM.Id, CM.TicketId, CM.SenderType, CM.SenderId, CM.Message, CM.CreatedDate, CM.ReadBy, CASE WHEN F.Id IS NOT NULL THEN 'File' ELSE 'Text' END AS MessageType, F.Id AS FileId, F.FileName, F.FilePath, F.FileSize, F.ContentType, L.FullName AS SenderName FROM TicketMessages CM LEFT JOIN TicketFiles F ON F.MessageId=CM.Id LEFT JOIN Logins L ON L.Id=CM.SenderId WHERE CM.TicketId='" & ticketId & "' ORDER BY CM.Id ASC")
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID Detail")
        Catch ex As Exception
            MessageError(True, ex.ToString)
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function LoginAccess(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
