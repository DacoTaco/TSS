<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditTask.aspx.cs" Inherits="TSS_ASPWebForms.EditTask" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TSS : Technical Service System</title>
    <link href="Content/bootstrap.css" rel="stylesheet" type="text/css"/>
    <link href="CSS/Style.css" rel="stylesheet" type="text/css" />
    <link href="CSS/EditTask.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/jquery-3.1.1.js"></script>
    <script src="Scripts/bootstrap.js"></script>
    <script src="Scripts/TSS.js"></script>
    <script src="Scripts/TSS-EditTask.js"></script>
    <link href="CSS/lightbox.css" rel="stylesheet" type="text/css"/>
    <script src="Scripts/lightbox.js"></script>

    <meta name="viewport" content="initial-scale=1.0, user-scalable=no" />
    <meta name="google" content="notranslate" />
</head>
<body>
    <div class="centerPage container-fluid" id="MainContainer" style="border-width:2px;border:solid;background-color:lightblue;">
        <form id="EditTaskForm" runat="server">
            <div id="EditTask">
                <div id="loadingTaskDiv" class="blocker" hidden="hidden" runat="server" style="z-index:999;">
                    <div class="blocker-content">
                        <asp:Image ID="loadingImage" runat="server" ImageAlign="AbsMiddle" ImageUrl="/system/loading.gif" style="width:3em;height:auto;z-index:9999;"/>
                    </div>
                </div>
                <!-- Show the read only msg if needed -->
                <script type="text/javascript" id="ReadOnly">
                    function myFunction() {
                        if ('True' == '<%# (ReadOnly) %>')
                                alert('<%# ReadOnlyMsg %>');
                    }
                </script>
                <table style="width:100%;table-layout:fixed;margin-bottom:10px;margin-right:auto;margin-left:auto">
                    <tr>
                        <!-- amount of columns -->
                        <th style="width:auto;"></th>
                        <th style="width:30%;"></th>
                        <th style="width:17%;"></th>
                        <th style="width:27%;"></th>
                    </tr>
                    <!-- columns! -->
                    <tr><!-- Reporter name & Technician row -->
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;"><asp:Label runat="server" Text="Reporter : " ID="lblReporter" meta:resourcekey="Reporter" Width="100%"></asp:Label></td>
                        <td style="text-align:left;padding-bottom:1em;padding-right:1em;height:5px;"><input type="text" runat="server" id="txtReporter" value="<%# Task.Reporter %>" autocomplete="off" onblur="updateTaskReporter(); return false;" style="width:100%;min-height:5px;height:2em;max-height:inherit;"/></td>
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;"><asp:Label runat="server" Text="Technician : " meta:resourcekey="Technician" Width="100%"></asp:Label></td>
                        <td style="text-align:left;padding-bottom:1em;padding-right:1em;"><select runat="server" id="selectTechnicians" style="width:100%" DataValueField="ID" DataTextField="UserName" onchange="updateTaskTechnician(); return false;"></select></td>
                    </tr>
                    <tr><!-- department & location row -->
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;"> <asp:Label runat="server" Text="Department : " meta:resourcekey="Department" Width="100%"></asp:Label> </td>
                        <td style="text-align:left;padding-bottom:1em;padding-right:1em;"><select runat="server" id="selectDepartments" style="width:100%;" DataValueField="ID" DataTextField="Name" onchange="updateTaskDepartment(); return false;"></select></td>
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;"> 
                            <asp:Label runat="server" Text="Location : " meta:resourcekey="Location" Width="100%"></asp:Label> 
                        </td>
                        <td style="text-align:left;padding-bottom:1em;padding-right:1em;">
                            <div id="locations">
                                <select runat="server" id="selectLocations" style="width:100%" DataValueField="ID" DataTextField="Name" onchange="updateTaskLocation(); return false;"></select>
                            </div>
                        </td>
                    </tr>
                    <tr><!-- Description row -->
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;"> <asp:Label runat="server" Text="Description : " meta:resourcekey="Description" Width="100%"></asp:Label> </td>
                        <td style="text-align:left;padding-bottom:1em;padding-right:1em;" colspan="3"><input type="text" runat="server" id="txtDescription" value="<%# Task.Description %>" onblur="updateTaskDescription(); return false;" autocomplete="off" style="width:100%;min-height:5px;height:2em;max-height:inherit;" /></td>
                    </tr>
                    <tr> <!-- Notes row -->
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;vertical-align:top;"> <asp:Label runat="server" Text="Notes : " meta:resourcekey="Notes" Width="100%"></asp:Label> </td>
                        <td style="text-align:left;padding-bottom:1em;padding-right:1em;width:100%;" colspan="3" rowspan="4" class="notesBox">
                            <asp:TextBox runat="server" ID="Notes" Width="100%" Text="<%# Task.strNotes %>" TextMode="MultiLine" Enabled="false" style="resize:none;min-height:inherit;max-height:inherit"></asp:TextBox>
                        </td>
                    </tr>
                    <tr><td></td></tr>
                    <tr><td></td></tr>
                    <tr><td></td></tr>
                    <tr><!-- add notes row -->
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;"> <asp:Label runat="server" Text="Add Notes : " meta:resourcekey="AddNotes"> </asp:Label> </td>
                        <td style="text-align:left;padding-bottom:1em;padding-right:1em;" colspan="2">
                             <input runat="server" id="NoteBox" autocomplete="off" style="min-height:5px;height:inherit;max-height:inherit;width:100%"/>
                        </td>
                        <td style="text-align:left;padding-bottom:1em;padding-right:1em;"> 
                            <asp:button runat="server" id="AddNotesButton" style="width:100%" OnClientClick="AddTaskNote(); return false;" value="Add Note" meta:resourcekey="AddNotesBtn">
                                
                            </asp:button>
                        </td>
                    </tr>
                    <!-- Machine row -->
                    <tr runat="server" id="machineRow" >
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;"> <asp:Label runat="server" Text="Machines : " meta:resourcekey="Machines"> </asp:Label> </td>
                        <td style="text-align:left;padding-bottom:1em;padding-right:1em;" colspan="2"> <select runat="server" id="selectMachines" style="width:100%" DataValueField="ID" datatextfield="Name" onchange="updateTaskMachine(); return false;"></select></td>
                        <td style="text-align:left;padding-bottom:1em;padding-right:1em;"> <asp:Button runat="server" Text="Documentation..." meta:resourcekey="Documentation" Width="100%" OnClientClick="alert('not yet implemented'); return false;"></asp:Button></td>
                    </tr>
                    <tr><!-- Task State row -->
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;"> <asp:Label runat="server" Text="State : " meta:resourcekey="TaskState"></asp:Label></td>
                        <td style="text-align:left;padding-bottom:1em;padding-right:1em;"> <select runat="server" id="selectTaskState" style="width:100%" DataValueField="ID" DataTextField="Status" onchange="updateTaskState(); return false;"></select></td>
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;"><asp:Label runat="server" Text="Urguent : " meta:resourcekey="Urguent"></asp:Label></td>
                        <td style="padding-bottom:1em;padding-right:1em;text-align:left;vertical-align:middle;"> 
                            <asp:CheckBox runat="server" ID="chkUrguent" onchange="updateTaskUrguency(); return false;" Checked="<%# Urguent %>" />

                        </td>
                    </tr>
                    <tr><!-- Photo's row -->
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;vertical-align:top;"> <asp:Label runat="server" Text="Photo's : " meta:resourcekey="Photos"></asp:Label></td>
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;height:100%;width:100%;max-height:12em;min-height:12em;" colspan="3" rowspan="4">
                            <div style="max-height:inherit;min-height:inherit;overflow:scroll;overflow-y:hidden;white-space:nowrap;background-color:white;border:solid;border-color:black;border-width:1px;padding:0px">
                                <!-- the list containing all photos -->
                                <ul runat="server" id="imagesList" style="max-height:10em;min-height:10em;width:100%;height:100%;float:left;padding:5px;display:inline;text-align:left;">

                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr><td></td></tr>
                    <tr><td></td></tr>
                    <tr><td></td></tr>
                    <tr> <!-- Add photos row -->
                        <td></td>
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;" colspan="2">
                            <asp:Button runat="server" ID="AddPhotoBtn" Text="Add Photo"  meta:resourcekey="AddPhoto" style="min-width:2em;width:60%;" 
                                OnClientClick="$('#AddPhotoInput').click(); return false;" />

                            <input id="AddPhotoInput" runat="server" type="file" accept="image/*" capture="camera" onchange="AddTaskPhoto(this); return false;" style="visibility:hidden"/> 
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" style="margin-left:auto;margin-right:auto;font-size:1em;">
                            <asp:Label runat="server" Text="Last Saved : " meta:resourcekey="LastAdjusted"></asp:Label>
                            <asp:Label runat="server" Text="<%# Task.LastAdjustment %>"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
        </form>
    </div>
</body>
</html>
