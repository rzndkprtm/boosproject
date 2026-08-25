Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Mounting_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/mounting", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("mountingid")) Then
            Response.Redirect("~/setting/specification/mounting", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("mountingid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If txtName.Text = "" Then
                MessageError(True, "MOUNTING NAME IS REQUIRED !")
                Exit Sub
            End If

            Dim blind As String = String.Empty
            For Each item As ListItem In lbBlind.Items
                If item.Selected Then
                    blind += item.Value & ","
                End If
            Next

            If blind = "" Then
                MessageError(True, "BLIND NAME IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Mountings ORDER BY Id DESC")
                Dim blindId As String = blind.Remove(blind.Length - 1).ToString()
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE Mountings SET Name=@Name, BlindId=@BlindId, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@BlindId", blindId)
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"Mountings", lblId.Text, Session("LoginId").ToString(), "Mounting Updated"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/specification/mounting", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/specification/mounting", False)
    End Sub

    Protected Sub BindData(mountingId As String)
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM Mountings WHERE Id='" & mountingId & "'")
            If myData Is Nothing Then
                Response.Redirect("~/setting/specification/mounting", False)
                Exit Sub
            End If

            BindBlind()

            txtName.Text = myData("Name").ToString()
            txtDescription.Text = myData("Description").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(myData("Active"))

            If Not myData("BlindId").ToString() = "" Then
                Dim blindArray() As String = myData("BlindId").ToString().Split(",")
                For Each i In blindArray
                    If Not (i.Equals(String.Empty)) Then
                        lbBlind.Items.FindByValue(i).Selected = True
                    End If
                Next
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindBlind()
        lbBlind.Items.Clear()
        Try
            lbBlind.DataSource = settingClass.GetDataTable("SELECT Blinds.Id, CONVERT(VARCHAR, Designs.Name) + ' | ' + CONVERT(VARCHAR, Blinds.Name) AS NameText FROM Blinds LEFT JOIN Designs ON Blinds.DesignId=Designs.Id WHERE Blinds.Active=1 ORDER BY Designs.Id, Blinds.Id ASC")
            lbBlind.DataTextField = "NameText"
            lbBlind.DataValueField = "Id"
            lbBlind.DataBind()

            If lbBlind.Items.Count > 0 Then
                lbBlind.Items.Insert(0, New ListItem("", ""))
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
