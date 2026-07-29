Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Product_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/product/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindDesignType()
            BindPriceGroup(ddlDesign.SelectedValue)
        End If
    End Sub

    Protected Sub ddlDesign_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindPriceGroup(ddlDesign.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlDesign.Text = "" Then
                MessageError(True, "PRODUCT / DESIGN TYPE IS REQUIRED !")
                Exit Sub
            End If

            If txtName.Text = "" Then
                MessageError(True, "NAME IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim priceGroupId As String = String.Empty
                If Not lbPriceGroup.SelectedValue = "" Then
                    priceGroupId = String.Join(",", lbPriceGroup.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) i.Value))
                End If

                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM PriceProductGroups ORDER BY Id DESC")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO PriceProductGroups VALUES (@Id, @Name, @DesignId, @PriceGroupId, @Description, @Status)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@DesignId", ddlDesign.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@PriceGroupId", priceGroupId)
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisCmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"ProductGroups", thisId, Session("LoginId").ToString(), "Price Product Group Created"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/price/product", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/product/", False)
    End Sub

    Protected Sub BindDesignType()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = settingClass.GetDataTable("SELECT * FROM Designs ORDER BY Name ASC")
            ddlDesign.DataTextField = "Name"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            If ddlDesign.Items.Count > 0 Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindPriceGroup(designId As String)
        lbPriceGroup.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(designId) Then
                Dim type As String = settingClass.GetItemData("SELECT Type FROM Designs WHERE Id='" & designId & "'")

                lbPriceGroup.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM PriceGroups WHERE Type='" & type & "' AND Status='Active' ORDER BY Name ASC")
                lbPriceGroup.DataTextField = "Name"
                lbPriceGroup.DataValueField = "Id"
                lbPriceGroup.DataBind()

                If lbPriceGroup.Items.Count > 0 Then
                    lbPriceGroup.Items.Insert(0, New ListItem("", ""))
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
