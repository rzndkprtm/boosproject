Imports System.Data.SqlClient

Partial Class Setting_General_Newsletter_Add
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/general/newsletter", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindComponentForm(ddlType.SelectedValue)
            BindCompany()
        End If
    End Sub

    Protected Sub ddlType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindComponentForm(ddlType.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If txtName.Text = "" Then
                MessageError(True, "NEWSLETTER NAME IS REQUIRED !")
                Exit Sub
            End If

            If ddlCompany.SelectedValue = "" Then
                MessageError(True, "COMPANY IS REQUIRED !")
                Exit Sub
            End If

            If ddlType.SelectedValue = "" Then
                MessageError(True, "NEWSLETTER TYPE IS REQUIRED !")
                Exit Sub
            End If

            If ddlType.SelectedValue = "Image" AndAlso Not fuFile.HasFile Then
                MessageError(True, "PLEASE UPLOAD YOUR IMAGE !")
                Exit Sub
            End If

            If ddlType.SelectedValue = "Link" AndAlso txtLink.Text = "" Then
                MessageError(True, "NEWSLETTER LINK IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Newsletters ORDER BY Id DESC")

                Dim link As String = String.Empty
                If ddlType.SelectedValue = "Link" Then link = txtLink.Text.Trim()
                If ddlType.SelectedValue = "Image" Then
                    Dim ext As String = IO.Path.GetExtension(fuFile.FileName)

                    Dim newFileName As String = Now.ToString("yyyyMMddHHmmss") & ext

                    Dim savePath As String = Server.MapPath("~/Assets/newsletter/" & newFileName)
                    fuFile.SaveAs(savePath)

                    link = String.Format("https://bigblinds.ordersblindonline.com/assets/newsletter/{0}", newFileName)
                    'link = String.Format("https://ordersblindonline.com/assets/newsletter/{0}", newFileName)
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Newsletters VALUES (@Id, @CompanyId, @Name, @Type, @Link, @Description, 0);", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@CompanyId", ddlCompany.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Link", link)
                        myCmd.Parameters.AddWithValue("@Description", txtDescription.Text)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Newsletters", thisId, Session("LoginId").ToString(), "Newsletter Created"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/general/newsletter", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/general/newsletter/", False)
    End Sub

    Protected Sub BindComponentForm(Type As String)
        Try
            divLink.Visible = False : divFile.Visible = False

            If Not String.IsNullOrEmpty(Type) Then
                If Type = "Image" Then divFile.Visible = True
                If Type = "Link" Then divLink.Visible = True
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            ddlCompany.DataSource = settingClass.GetDataTable("SELECT * FROM Companys ORDER BY Name ASC")
            ddlCompany.DataTextField = "Name"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            If ddlCompany.Items.Count > 1 Then
                ddlCompany.Items.Insert(0, New ListItem("", ""))
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
