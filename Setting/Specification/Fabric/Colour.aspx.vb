Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Fabric_Colour
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/fabric", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnChangeStatus_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdStatus.Text
            Dim newStatus As String = ddlNewStatus.SelectedValue
            Dim oldStatus As String = ddlOldStatus.SelectedValue

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE FabricColours SET Status=@Status WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@Status", newStatus)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim changeDesc As String = String.Format("Change Status Fabric Colour : {0}", newStatus)
            dataLog = {"FabricColours", thisId, Session("LoginId").ToString(), changeDesc}
            settingClass.Logs(dataLog)

            Dim aliasData As DataRow = settingClass.GetDataRow("SELECT SecondId FROM FabricAlias WHERE Type='FabricColours' AND FirstId='" & thisId & "'")
            If aliasData IsNot Nothing Then
                Dim aliasId As String = aliasData(0).ToString()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE FabricColours SET Status=@Status WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", aliasId)
                        myCmd.Parameters.AddWithValue("@Status", newStatus)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                changeDesc = String.Format("Change Status Fabric Colour : {0}", newStatus)
                dataLog = {"FabricColours", aliasId, Session("LoginId").ToString(), changeDesc}
                settingClass.Logs(dataLog)
            End If

            Response.Redirect("~/setting/specification/fabric/colour", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Try
            Dim searchString As String = String.Empty
            If Not String.IsNullOrEmpty(searchText) Then
                searchString = "WHERE Fabrics.Name LIKE '%" & searchText & "%' OR FabricColours.Colour LIKE '%" & searchText & "%'"
            End If
            Dim thisString As String = String.Format("SELECT FabricColours.*, Fabrics.Name AS FabricName FROM FabricColours LEFT JOIN Fabrics ON FabricColours.FabricId=Fabrics.Id {0} ORDER BY FabricColours.Name ASC", searchString)

            gvList.DataSource = settingClass.GetDataTable(thisString)
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
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
