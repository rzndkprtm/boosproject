Imports System.Data.SqlClient

Partial Class Setting_Price_Base_Markup
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/base", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindPriceGroup()
            BindPriceGroup(ddlPriceGroup.SelectedValue)
        End If
    End Sub

    Protected Sub ddlPriceGroup_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindPriceGroup(ddlPriceGroup.SelectedValue)
    End Sub

    Protected Sub btnSubmitAgain_Click(sender As Object, e As EventArgs)
        Process("Add")
    End Sub

    Protected Sub btnSubmitFinish_Click(sender As Object, e As EventArgs)
        Process()
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/base", False)
    End Sub

    Protected Sub Process(Optional action As String = "")
        MessageError(False, String.Empty)
        Try
            If ddlPriceGroup.SelectedValue = "" Then
                MessageError(True, "PRICE GROUP (FROM) IS REQUIRED !")
                Exit Sub
            End If
            If ddlCategory.SelectedValue = "" Then
                MessageError(True, "CATEGORY IS REQUIRED !")
                Exit Sub
            End If
            If lbProductGroup.SelectedValue = "" Then
                MessageError(True, "PRODUCT GROUP IS REQUIRED !")
                Exit Sub
            End If
            If ddlType.SelectedValue = "" Then
                MessageError(True, "MARKUP TYPE IS REQUIRED !")
                Exit Sub
            End If
            If txtMarkup.Text = "" Then
                MessageError(True, "MARKUP VALUE IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                If ddlBackup.SelectedValue = "Yes" Then
                    Dim newTable As String = "PriceBases_Backup_" & "_" & Session("RoleName").ToString() & DateTime.Now.ToString("yyyyMMdd_HHmmss")
                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As New SqlCommand("SELECT * INTO [dbo].[" & newTable & "] FROM [dbo].[PriceBases]", thisConn)
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using
                End If

                Dim productGroup As String = String.Empty
                If Not lbProductGroup.SelectedValue = "" Then
                    productGroup = String.Join(",", lbProductGroup.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) i.Value))
                End If

                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@PriceGroupId", ddlPriceGroup.SelectedValue),
                    New SqlParameter("@Category", ddlCategory.SelectedValue),
                    New SqlParameter("@ProductGroupId", If(String.IsNullOrEmpty(productGroup), CType(DBNull.Value, Object), productGroup)),
                    New SqlParameter("@Markup", txtMarkup.Text),
                    New SqlParameter("@MarkupType", ddlType.SelectedValue)
                }
                settingClass.ExecuteSP("sp_PriceBases_Markup", params)

                Dim url As String = "~/setting/price/base"
                If action = "Add" Then url = "~/setting/price/base/markup"
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindPriceGroup()
        ddlPriceGroup.Items.Clear()
        Try
            Dim thisString As String = "SELECT Id, Name FROM PriceGroups WHERE Type='Blinds' AND (Status='Active' OR Status='Inactive') ORDER BY Id ASC"

            ddlPriceGroup.DataSource = settingClass.GetDataTable(thisString)
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

    Protected Sub BindPriceGroup(priceGroupId As String)
        lbProductGroup.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(priceGroupId) Then
                Dim thisQuery As String = "SELECT Id, Name FROM PriceProductGroups CROSS APPLY STRING_SPLIT(PriceGroupId, ',') AS thisArray WHERE thisArray.VALUE='" & priceGroupId & "' AND Status='Active' ORDER BY Name ASC"

                lbProductGroup.DataSource = settingClass.GetDataTable(thisQuery)
                lbProductGroup.DataTextField = "Name"
                lbProductGroup.DataValueField = "Id"
                lbProductGroup.DataBind()

                If lbProductGroup.Items.Count > 0 Then
                    lbProductGroup.Items.Insert(0, New ListItem("", ""))
                End If
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
