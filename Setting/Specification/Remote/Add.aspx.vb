Imports System.Data.SqlClient

Partial Class Setting_Specification_Remote_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/chain", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindDesign()
            BindControl()
            BindCompanyDetail()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If Not String.IsNullOrEmpty(txtBoeId.Text) Then
                If Not IsNumeric(txtBoeId.Text) Then
                    MessageError(True, "BOE ID SHOULD BE NUMERIC !")
                    Exit Sub
                End If
            End If
            If txtName.Text = "" Then
                MessageError(True, "REMOTE NAME IS REQUIRED !")
                Exit Sub
            End If
            If lbDesign.SelectedValue = "" Then
                MessageError(True, "DESIGN TYPE IS REQUIRED !")
                Exit Sub
            End If
            If lbControl.SelectedValue = "" Then
                MessageError(True, "CONTROL TYPE IS REQUIRED !")
                Exit Sub
            End If

            If lbCompanyDetail.SelectedValue = "" Then
                MessageError(True, "SUB COMPANY IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim selectedDesign As String = String.Empty
                For Each item As ListItem In lbDesign.Items
                    If item.Selected Then
                        selectedDesign += item.Value & ","
                    End If
                Next

                Dim selectedControl As String = String.Empty
                For Each item As ListItem In lbControl.Items
                    If item.Selected Then
                        selectedControl += item.Value & ","
                    End If
                Next

                Dim selectedCompany As String = String.Empty
                For Each item As ListItem In lbCompanyDetail.Items
                    If item.Selected Then
                        selectedCompany += item.Value & ","
                    End If
                Next

                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Chains ORDER BY Id DESC")
                Dim designType As String = selectedDesign.Remove(selectedDesign.Length - 1).ToString()
                Dim controlType As String = selectedControl.Remove(selectedControl.Length - 1).ToString()
                Dim companyDetail As String = selectedCompany.Remove(selectedCompany.Length - 1).ToString()
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO Chains VALUES (@Id, @BoeId, @Name, @DesignId, @ControlTypeId, @CompanyDetailId, NULL, NULL, @Description, @Status)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@BoeId", If(String.IsNullOrEmpty(txtBoeId.Text), CType(DBNull.Value, Object), txtBoeId.Text))
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@DesignId", designType)
                        thisCmd.Parameters.AddWithValue("@CompanyDetailId", companyDetail)
                        thisCmd.Parameters.AddWithValue("@ControlTypeId", controlType)
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisCmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"Chains", thisId, Session("LoginId").ToString(), "Remote Created"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/specification/remote", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/specification/remote", False)
    End Sub

    Protected Sub BindDesign()
        lbDesign.Items.Clear()
        Try
            lbDesign.DataSource = settingClass.GetDataTable("SELECT * FROM Designs CROSS APPLY STRING_SPLIT(AppliesTo, ',') applyArray WHERE applyArray.VALUE='Chains' ORDER BY Name ASC")
            lbDesign.DataTextField = "Name"
            lbDesign.DataValueField = "Id"
            lbDesign.DataBind()

            If lbDesign.Items.Count > 0 Then
                lbDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindControl()
        lbControl.Items.Clear()
        Try
            lbControl.DataSource = settingClass.GetDataTable("SELECT * FROM ProductControls WHERE Type='Motorised' ORDER BY Name ASC")
            lbControl.DataTextField = "Name"
            lbControl.DataValueField = "Id"
            lbControl.DataBind()

            If lbControl.Items.Count > 0 Then
                lbControl.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompanyDetail()
        lbCompanyDetail.Items.Clear()
        Try
            lbCompanyDetail.DataSource = settingClass.GetDataTable("SELECT * FROM CompanyDetails WHERE Status='Active' OR Status='Inactive' ORDER BY Name ASC")
            lbCompanyDetail.DataTextField = "Name"
            lbCompanyDetail.DataValueField = "Id"
            lbCompanyDetail.DataBind()

            If lbCompanyDetail.Items.Count > 0 Then
                lbCompanyDetail.Items.Insert(0, New ListItem("", ""))
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
