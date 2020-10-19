using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 登录的json数据格式
/// </summary>
public class LoginJson
{
    public string username;
    public string password;
}
/// <summary>
/// 用户信息
/// </summary>
public class UserInfo
{
    public string userCode;
    public string userId;
    public string orgName;
    public string userNameCn;
    public string orgNameEn;
}
/// <summary>
/// 发送考试结果
/// </summary>
public class SendResult
{
    public string vrName= "技术培训_新能源_⾼压安全强化及G38PHEV电池模组拆装";
    //"userCode": "hzz03",
    public string userCode;
    //"userId": "1",
    public string userId;

    //"modelType": 0,
    public int modelType;

    //"examDate": "2020-01-05 16:12:52",
    public string examDate;
    //"examScores": 95,
    public int examScores;
    public List<ResultInfo> examResult;
    //"examResult": [{
    // "key": "T0",
    // "value": 0
    //}, {
    // "key": "T1",
    // "value": 1
    //}, {
    // "key": "T2",
    // "value": 2
    //}, {
    // "key": "T3",
    // "value": 3
    //}, {
    // "key": "T4",
    // "value": 4
    //}]
}
/// <summary>
/// 考试结果
/// </summary>
[SerializeField]
public class ResultInfo
{
    [SerializeField]
    public string key;
    [SerializeField]
    public int value;
}

/// <summary>
/// 登录结果
/// </summary>
public class LoginResult
{
    public string msg;
    public int code;
    public LoginData data;
}
/// <summary>
/// 登录结果
/// </summary>
public class LoginData
{
    public int jobStatus;
    public string nameCn;
    public string userAgent;
    public string accessToken;
    public string userId;
    public string token;
    public int roleTypeVal;
    public string cardId;
    public string expire;
    public string name;
    public string userIp;
    public int curriculumVitaeCode;
    public string position;
    public UserLoginInfo userLoginInfo;
}
/// <summary>
/// 用户信息的结果
/// </summary>
public class UserLoginInfo
{
    public string emergencyContactInfo;
    public string enterSchoolDate;
    public string privacyFileUrl;
    public string availabilityDate;
    public string jobCode;
    public string loginFlag;
    public string lastNameCn;
    public string partManagerThreeId;
    public string partTimeJobcodeThree;
    public string userCardNo;
    public string orgType;
    public string teacherRole;
    public string currentVersionDate;
    public string partManagerTwoId;
    public string partTimeOutletTwo;
    public string bmwDeptCode;
    public string jobcodeLeaveDate;
    public string jobcodeAge;
    public string jobcodeJoinDateTwo;
    public string officialAccountFlag;
    public string idType;
    public string bmwSeniority;
    public string partManagerOneId;
    public string teacherRoleEn;
    public string version;
    public string firstName;
    public string firstNameEn;
    public string talentBankFlag;
    public string teacherGroupCode;
    public string uploadHost;
    public string directManagerId;
    public string partTimeJobcodeFour;
    public string userType;
    public string jobcodeJoinDate;
    public string leaveSchoolDate;
    public string networkJoinDate;
    public string birthday;
    public string lastName;
    public string jobStatus;
    public string bmwJoinDate;
    public string educationCode;
    public string jobcodeLeaveDateOne;
    public string regionIds;
    public string dealerShipId;
    public string delFlag;
    public string lastNameEn;
    public string orgId;
    public string userCode;
    public string userNameCn;
    public string jobcodeLeaveDateFour;
    public string orgJoinDate;
    public string teacherType;
    public string agreePolicy;
    public string graduateStatus;
    public string authType;
    public string jobcodeJoinDateThree;
    public string majorCode;
    public string email;
    public string registrationApprovaStatus;
    public string jobcodeJoinDateFour;
    public string headurl;
    public string mobile;
    public string teacherRoleCn;
    public string updateUser;
    public string updateTime;
    public string userAgent;
    public string kill;
    public string userLoginIP;
    public string userName;
    public string partTimeJobcodeTwo;
    public string userId;
    public string jobLevel;
    public string teacherGrade;
    public string firstNameCn;
    public string jobcodeLeaveDateTwo;
    public string userNameEn;
    public string jobcodeJoinDateOne;
    public string lastLoginTime;
    public string jobcodeLeaveDateThree;
    public string sexCode;
    public string partManagerFourId;
    public string partTimeJobcodeOne;
    public string partTimeOutletOne;
    public string loginFailCount;
    public string trialEndDate;
    public string createTime;
    public string roleTypeVal;
    public string idcard;
    public string leaveDate;
    public string annualTrainingObj;
    public string loginFailTime;
    public string createUser;
    public string idPhoto;
    public string contractCode;
    public string superAdmin;
    public string passwordUpdateTime;
}
