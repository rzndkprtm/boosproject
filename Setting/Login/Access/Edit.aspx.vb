Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Partial Class Setting_Login_Access_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/login/access", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("accessid")) Then
            Response.Redirect("~/setting/login/access", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("accessid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlRoleId.SelectedValue = "" Then
                MessageError(True, "ROLE NAME IS REQUIRED !")
                Exit Sub
            End If
            If ddlLevelId.SelectedValue = "" Then
                MessageError(True, "LEVEL ACCESS IS REQUIRED !")
                Exit Sub
            End If
            If ddlPage.SelectedValue = "" Then
                MessageError(True, "PAGE IS REQUIRED !")
                Exit Sub
            End If
            If txtAction.Text = "" Then
                MessageError(True, "ACTION IS REQUIRED !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE LoginAccess SET RoleId=@RoleId, LevelId=@LevelId, Page=@Page, Action=@Action, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@RoleId", ddlRoleId.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@LevelId", ddlLevelId.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Page", ddlPage.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Action", txtAction.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Response.Redirect("~/setting/login/access", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/login/access", False)
    End Sub

    Protected Sub BindData(accessId As String)
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM LoginAccess WHERE Id='" & accessId & "'")
            If myData Is Nothing Then Exit Sub

            BindRole()
            BindLevel()
            BindPage()

            ddlRoleId.SelectedValue = myData("RoleId").ToString()
            ddlLevelId.SelectedValue = myData("LevelId").ToString()
            ddlPage.SelectedValue = myData("Page").ToString()
            txtAction.Text = myData("Action").ToString()
            txtDescription.Text = myData("Description").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(myData("Active"))
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindRole()
        ddlRoleId.Items.Clear()
        Try
            ddlRoleId.DataSource = settingClass.GetDataTable("SELECT * FROM LoginRoles ORDER BY Name ASC")
            ddlRoleId.DataTextField = "Name"
            ddlRoleId.DataValueField = "Id"
            ddlRoleId.DataBind()

            If ddlRoleId.Items.Count > 1 Then
                ddlRoleId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindLevel()
        ddlLevelId.Items.Clear()
        Try
            ddlLevelId.DataSource = settingClass.GetDataTable("SELECT * FROM LoginLevels ORDER BY Name ASC")
            ddlLevelId.DataTextField = "Name"
            ddlLevelId.DataValueField = "Id"
            ddlLevelId.DataBind()

            If ddlLevelId.Items.Count > 1 Then
                ddlLevelId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindPage()
        Try
            ddlPage.Items.Clear()

            Dim rootPath As String = Server.MapPath("~/")

            Dim ignoreFolders As New List(Of String) From {"\bin\", "\obj\", "\App_Data\", "\App_Code\", "\Scripts\", "\Content\", "\Images\", "\fonts\", "\packages\", "\Properties\"}

            Dim regex As New Regex("Title\s*=\s*""([^""]*)""", RegexOptions.IgnoreCase)

            Dim pageList As New List(Of ListItem)

            For Each file As String In Directory.EnumerateFiles(rootPath, "*.aspx", SearchOption.AllDirectories)
                Try
                    Dim skip As Boolean = False

                    For Each folder As String In ignoreFolders
                        If file.IndexOf(folder, StringComparison.OrdinalIgnoreCase) >= 0 Then
                            skip = True
                            Exit For
                        End If
                    Next

                    If skip Then Continue For

                    Dim content As String = IO.File.ReadAllText(file)
                    Dim match As Match = regex.Match(content)

                    Dim title As String

                    If match.Success Then
                        title = match.Groups(1).Value.Trim()
                    Else
                        title = Path.GetFileNameWithoutExtension(file)
                    End If

                    pageList.Add(New ListItem(title))
                Catch
                    Continue For
                End Try
            Next
            pageList = pageList.OrderBy(Function(x) x.Text).ToList()
            ddlPage.Items.AddRange(pageList.ToArray())
        Catch ex As Exception
            MessageError(True, ex.Message)
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
