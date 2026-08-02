Imports System.Data.SqlClient

Partial Class Setting_Specification_Design_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/design", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindCompany()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If txtName.Text = "" Then
                MessageError(True, "NAME IS REQUIRED !")
                Exit Sub
            End If
            If ddlType.SelectedValue = "" Then
                MessageError(True, "TYPE IS REQUIRED !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Dim company As String = String.Empty
                Dim applyTo As String = String.Empty

                If Not lbCompany.SelectedValue = "" Then
                    company = String.Join(",", lbCompany.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) i.Value))
                End If
                If Not lbApplies.SelectedValue = "" Then
                    applyTo = String.Join(",", lbApplies.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) i.Value))
                End If

                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Designs ORDER BY Id DESC")
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO Designs VALUES (@Id, @Name, @Alias, @CompanyId, @Type, @Page, @AppliesTo, @Description, @Active)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Alias", txtAlias.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@CompanyId", company)
                        thisCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Page", txtPage.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@AppliesTo", applyTo)
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"Designs", thisId, Session("LoginId").ToString(), "Created"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/specification/design", False)

                'Using thisConn As New SqlConnection(myConn)
                '    Using thisCmd As SqlCommand = New SqlCommand("UPDATE Designs SET Name=@Name, Alias=@Alias, CompanyId=@CompanyId, Type=@Type, Page=@Page, AppliesTo=@AppliesTo, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                '        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                '        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                '        thisCmd.Parameters.AddWithValue("@Alias", txtAlias.Text.Trim())
                '        thisCmd.Parameters.AddWithValue("@CompanyId", company)
                '        thisCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                '        thisCmd.Parameters.AddWithValue("@Page", txtPage.Text.Trim())
                '        thisCmd.Parameters.AddWithValue("@AppliesTo", applyTo)
                '        thisCmd.Parameters.AddWithValue("@Description", descText)
                '        thisCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)
                '        thisConn.Open()
                '        thisCmd.ExecuteNonQuery()
                '    End Using
                'End Using

                'dataLog = {"Designs", lblId.Text, Session("LoginId").ToString(), "Updated"}
                'settingClass.Logs(dataLog)

                'Response.Redirect("~/setting/specification/design", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/specification/design", False)
    End Sub

    Protected Sub BindCompany()
        lbCompany.Items.Clear()
        Try
            lbCompany.DataSource = settingClass.GetDataTable("SELECT * FROM Companys WHERE Active=1 ORDER BY Name ASC")
            lbCompany.DataTextField = "Alias"
            lbCompany.DataValueField = "Id"
            lbCompany.DataBind()

            If lbCompany.Items.Count > 0 Then
                lbCompany.Items.Insert(0, New ListItem("", ""))
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
