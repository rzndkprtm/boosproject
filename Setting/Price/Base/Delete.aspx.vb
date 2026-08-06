
Imports System.Data.SqlClient
Imports System.Drawing

Partial Class Setting_Price_Base_Delete
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Dim pageAccess As Boolean = LoginAccess("Load")
        'If pageAccess = False Then
        '    Response.Redirect("~/setting/price/base", False)
        '    Exit Sub
        'End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindPriceGroup()
            BindProductGroup(ddlPriceGroup.SelectedValue)
        End If
    End Sub

    Protected Sub ddlPriceGroup_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindProductGroup(ddlPriceGroup.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If msgError.InnerText = "" Then
                Dim thisQuery As String = "DELETE FROM PriceBases WHERE PriceGroupId=@PriceGroupId AND ProductGroupId=@ProductGroupId AND Category=@Category"
                If ddlCategory.SelectedValue = "Sell & Buy" Then
                    thisQuery = "DELETE FROM PriceBases WHERE PriceGroupId=@PriceGroupId AND ProductGroupId=@ProductGroupId"
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand(thisQuery, thisConn)
                        thisCmd.Parameters.AddWithValue("@PriceGroupId", ddlPriceGroup.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@ProductGroupId", ddlProductGroup.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Response.Redirect("~/setting/price/base", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/base", False)
    End Sub

    Protected Sub BindPriceGroup()
        ddlPriceGroup.Items.Clear()
        Try
            ddlPriceGroup.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM PriceGroups WHERE Status='Active' ORDER BY Name ASC")
            ddlPriceGroup.DataTextField = "Name"
            ddlPriceGroup.DataValueField = "Id"
            ddlPriceGroup.DataBind()

            If ddlPriceGroup.Items.Count > 0 Then
                ddlPriceGroup.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlPriceGroup.Items.Clear()
        End Try
    End Sub

    Protected Sub BindProductGroup(priceGroupId As String)
        ddlProductGroup.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(priceGroupId) Then
                Dim query As String = "SELECT PriceProductGroups.Id, PriceProductGroups.Name FROM PriceProductGroups CROSS APPLY STRING_SPLIT(PriceGroupId, ',') AS thisArray WHERE thisArray.VALUE='" & priceGroupId & "'"

                ddlProductGroup.DataSource = settingClass.GetDataTable(query)
                ddlProductGroup.DataTextField = "Name"
                ddlProductGroup.DataValueField = "Id"
                ddlProductGroup.DataBind()

                If ddlProductGroup.Items.Count > 0 Then
                    ddlProductGroup.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            ddlProductGroup.Items.Clear()
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
