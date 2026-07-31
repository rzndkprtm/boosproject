Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Group_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/group/", False)
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
                MessageError(True, "PRICE GROUP NAME IS REQUIRED !")
                Exit Sub
            End If

            Dim checkData As DataRow = settingClass.GetDataRow("SELECT * FROM PriceGroups WHERE Name='" & txtName.Text.Trim().ToUpper() & "'")
            If checkData IsNot Nothing Then
                MessageError(True, "PRICE GROUP NAME ALREADY EXISTS !")
                Exit Sub
            End If

            If ddlCompany.SelectedValue = "" Then
                MessageError(True, "COMPANY IS REQUIRED !")
                Exit Sub
            End If

            If ddlType.SelectedValue = "" Then
                MessageError(True, "TYPE IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM PriceGroups ORDER BY Id DESC")
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO PriceGroups VALUES (@Id, @Name, @CompanyId, @Type, @Description, @Status)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim().ToUpper())
                        thisCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@CompanyId", ddlCompany.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisCmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"PriceGroups", thisId, Session("LoginId").ToString(), "Price Group Created"}
                settingClass.Logs(dataLog)

                lblId.Text = thisId
                lblType.Text = ddlType.SelectedValue

                gvProductGroup.DataSource = settingClass.GetDataTable("SELECT PriceProductGroups.Id, PriceProductGroups.Name FROM PriceProductGroups INNER JOIN Designs ON PriceProductGroups.DesignId = Designs.Id WHERE Designs.Type='" & lblType.Text & "'")
                gvProductGroup.DataBind()

                Dim thisScript As String = "window.onload = function() { showProductGroup(); };"
                ClientScript.RegisterStartupScript(Me.GetType(), "showProductGroup", thisScript, True)
                Exit Sub
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/group/", False)
    End Sub

    Protected Sub btnClose_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/group", False)
    End Sub

    Protected Sub btnProductGroup_Click(sender As Object, e As EventArgs)
        Try
            Dim selectedIds As New List(Of String)
            For Each row As GridViewRow In gvProductGroup.Rows
                Dim chk As CheckBox = CType(row.FindControl("chkSelect"), CheckBox)

                If chk IsNot Nothing AndAlso chk.Checked Then
                    selectedIds.Add(gvProductGroup.DataKeys(row.RowIndex).Value.ToString())
                End If
            Next

            If selectedIds.Count > 0 Then
                Dim sql As String = "UPDATE PriceProductGroups SET PriceGroupId = CASE WHEN PriceGroupId IS NULL OR PriceGroupId = '' THEN @Value WHEN ',' + PriceGroupId + ',' NOT LIKE '%,' + @Value + ',%' THEN PriceGroupId + ',' + @Value ELSE PriceGroupId END WHERE Id IN (" & String.Join(",", selectedIds) & ")"
                Using con As New SqlConnection(myConn)
                    Using cmd As New SqlCommand(sql, con)
                        cmd.Parameters.AddWithValue("@Value", lblId.Text)
                        con.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using
            End If

            Response.Redirect("~/setting/price/group", False)
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            ddlCompany.DataSource = settingClass.GetDataTable("SELECT Id, Alias FROM Companys WHERE Active=1 ORDER BY Id ASC")
            ddlCompany.DataTextField = "Alias"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            If ddlCompany.Items.Count > 0 Then
                ddlCompany.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCompany.Items.Clear()
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
