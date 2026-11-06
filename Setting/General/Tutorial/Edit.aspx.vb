Imports System.Data.SqlClient
Imports System.IO

Partial Class Setting_General_Tutorial_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/general/tutorial", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("tutorialid")) Then
            Response.Redirect("~/setting/general/tutorial", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("tutorialid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindCompany()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Dim savedFilePath As String = String.Empty
        Dim fileExtension As String = String.Empty
        Try
            If ddlCompanyId.SelectedValue = "" Then
                MessageError(True, "COMPANY IS REQUIRED !")
                Exit Sub
            End If
            If txtTitle.Text.Trim() = "" Then
                MessageError(True, "TITLE IS REQUIRED !")
                Exit Sub
            End If
            If fuFile.HasFile Then
                Dim allowedExtensions As String() = {".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".jpg", ".jpeg", ".png", ".mp4", ".webm"}
                fileExtension = Path.GetExtension(fuFile.FileName).ToLower()

                If Not allowedExtensions.Contains(fileExtension) Then
                    MessageError(True, "INVALID FILE FORMAT !")
                    Exit Sub
                End If
            End If

            If msgError.InnerText = "" Then
                Dim urlFile As String = String.Empty

                If fuFile.HasFile Then
                    Dim folderPath As String = Server.MapPath("~/File/Tutorial/")

                    If Not Directory.Exists(folderPath) Then
                        Directory.CreateDirectory(folderPath)
                    End If

                    Dim fileTitle As String = txtTitle.Text.Trim()

                    For Each invalidChar As Char In Path.GetInvalidFileNameChars()
                        fileTitle = fileTitle.Replace(invalidChar, "_"c)
                    Next

                    Dim dateTimeNow As String = DateTime.Now.ToString("dd MM yyyy hh mm ss")
                    Dim fileName As String = fileTitle & " " & dateTimeNow & fileExtension
                    Dim physicalPath As String = Path.Combine(folderPath, fileName)
                    Dim counter As Integer = 1

                    While File.Exists(physicalPath)
                        fileName = fileTitle & " " & dateTimeNow & " (" & counter.ToString() & ")" & fileExtension
                        physicalPath = Path.Combine(folderPath, fileName)
                        counter += 1
                    End While

                    fuFile.SaveAs(physicalPath)

                    urlFile = "~/File/Tutorial/" & fileName
                    savedFilePath = physicalPath
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("UPDATE Tutorials SET CompanyId=@CompanyId, Title=@Title, Description=@Description, Video=@Video, Status=@Status WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@CompanyId", ddlCompanyId.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@File", urlFile)
                        thisCmd.Parameters.AddWithValue("@Video", txtVideo.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)

                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Tutorials", lblId.Text, Session("LoginId").ToString(), "Tutorial Udpated"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/general/tutorial", False)
                HttpContext.Current.ApplicationInstance.CompleteRequest()
            End If
        Catch ex As Exception
            If savedFilePath <> String.Empty AndAlso File.Exists(savedFilePath) Then
                File.Delete(savedFilePath)
            End If
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/general/tutorial", False)
    End Sub

    Protected Sub BindCompany()
        ddlCompanyId.Items.Clear()
        Try
            ddlCompanyId.DataSource = settingClass.GetDataTable("SELECT * FROM Companys WHERE Status='Active' ORDER BY Name ASC")
            ddlCompanyId.DataTextField = "Alias"
            ddlCompanyId.DataValueField = "Id"
            ddlCompanyId.DataBind()

            If ddlCompanyId.Items.Count > 1 Then
                ddlCompanyId.Items.Insert(0, New ListItem("", ""))
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
