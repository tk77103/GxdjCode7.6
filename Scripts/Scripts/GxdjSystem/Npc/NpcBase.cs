using System.Collections.Generic;
#region 属性相关枚举声明
public enum FriendshipTag
{
    死敌,
    憎恨,
    厌恶,
    陌生,
    泛泛,
    朋友,
    知己,
    死生
}
public class ForIdRecord : BaseManger<ForIdRecord>
{
    public short Id = 1000;
    private ForIdRecord() { }
}
/// <summary>
/// 静态的字母类只存储字母
/// </summary>
public class LetterData:BaseManger<LetterData>
{
    public char[] letters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K' };
    private LetterData() { }
}
/// <summary>
/// 民族
/// </summary>
public enum Nation
{
    夏川 = 0,
    樱山 = 1,
    歌铎 = 2,
    亚蓝 = 3,
    迦梨 = 4,
    泰马 = 5,
}
/// <summary>
/// 星座
/// </summary>
public enum Constellation
{
    水瓶 = 0,
    双鱼 = 1,
    白羊 = 2,
    金牛 = 3,
    双子 = 4,
    巨蟹 = 5,
    狮子 = 6,
    处女 = 7,
    天秤 = 8,
    天蝎 = 9,
    射手 = 10,
    摩羯 = 11,
}
/// <summary>
/// npc与主角的亲属关系
/// </summary>
public enum Kinship
{
    无关 = 0, 
    父亲 = 1, 
    母亲 = 2, 
    儿子 = 3, 
    女儿 = 4, 
    兄弟 = 5, 
    姐妹 = 6, 
    兄妹 = 7, 
    配偶 = 8, 
    恋人 = 9, 
    前夫 = 10, 
    前妻 = 11, 
    前任 = 12
}
/// <summary>
/// 物品的分类枚举
/// </summary>
public enum ItemSort
{
    珠宝 = 0,
    珍宝 = 1,
    手办 = 2,
    画作 = 3
}
public enum Degree
{
    初中,
    高中,
    学士,
    硕士,
    博士
}
public class NpcRelationships
{
    public short relatedID;
    //isPersonal私人关系 0 无关 1 父亲 2 母亲 3 儿子 4 女儿 5 兄弟 6 姐妹 7 兄妹 8 配偶   9恋人 10前夫 11 前妻 12 前任
    public Kinship personalRelations;
    //isPrivate 0 无关 1 情人  
    public short romanticRelations;
    // 同事关系 0 无关 1下级 2 同级 3 上级
    public short colleagueRelations;
    // personal友情关系 -100 -50 -20 0 10 30 95 100 死敌 憎恨 厌恶  陌生 泛泛 朋友  知己 生死）
    public int friendship=0;
    public FriendshipTag friendshipTag;
    //此处藏点私货，因为觉得目前的设计未来还是要这么改 情感关系
    public short loveship=0;    
    //谈话次数
    public int isTalkedThree = 0;
}
#endregion
public class NpcBase
{
    #region 数据容器
    #region 基础信息
    //序号 1000-2999顺序生成
    public short Id;
    //生理性别 男性，女性 0 1
    public bool gender;
    //姓名会分成12个name_list，分别对应泰马人、夏川人、樱山人、歌铎人、亚蓝人、迦梨人6个民族以及2个性别的姓名表。其中包含姓名及对应的英文名、日文名以便于之后进行多语言本地化。其中樱山、夏川两个民族要考虑随机出姓名搭配，其他民族不需要。
    public string name;
    //当今年龄 读取scene_list 随机值
    public short npcAge;
    //personal0=夏川 1=樱山 2=歌铎 3=亚蓝 4=迦梨 5=泰马
    public Nation race;
    //personal1=01.水瓶，2=02.双鱼，3=03.白羊，4=04.金牛，5=05.双子，6=06.巨蟹，7=07.狮子，8=08.处女，9=09.天秤，10=10.天蝎，11=11.射手，12=12.摩羯
    public Constellation zodiac;
    //是否怀孕 判定是否怀孕的状态
    public bool pregnance;
    //npc舌战体力值上限
    public int debateHP;
    #endregion
    #region 智识能力
    //教育程度
    public Degree degree;
    //智识等级 主要的人物等级，读取scene_list，取值范围(0,100)
    public float intellegence;
    //npc成长类型
    public short growType;
    //是否停止生成
    public bool isStopGrow = false;
    //npc工作技能
    public JobSkill myJobSkill=new();
    #endregion
    #region 工作相关
    #region 薪资相关
    //npc的工作等级
    public short jobLevel;
    //所在行业
    public string industry;
    //职称
    public string jobTitle;
    //npc的基础薪资与行业工资相匹配
    public float basicSalary;
    //npc实际工资
    public float actualSalary;
    #endregion

    //所属企业
    public string company;


    //售卖列表
    public List<short> sellLIst = new List<short>();
    //购入列表
    public List<short> buyList = new List<short>();
    //personal职场关系 全部同事的关系值平均
    public float workRelAvr;
    //personal高层关系 高于自己职务的全部人员关系值平均
    public float leaderRelAvr;
    //personal职场影响 等级*职场关系
    public float workInfluence;
    //所在公司学术倾向
    public string companyAcademy;
    //是否在职场公司
    public bool isWorkplace;
    #endregion
    #region 位置信息
    //默认出现场景  NPC默认出现的场景,从scene_list里面进行匹配
    public string defaultScene;
    //private所在地
    public string location;
    //所属企业所在区域
    public string companyArea;
    #endregion
    #region 个性相关
    //投资倾向 投资倾向标签 投资倾向 保守 谨慎 中立  大胆 激进 -100 50 0 50 100
    public short investTendency;
    public string investTendencyTag;
    //personal数值+道德标签显示 邪恶(-100,-71)，贪婪(-70,-30)，中庸(-31,30)，善良(31,70)，正直(71,100) 根据道德水准取值
    public short moral;

    public int moralTag;
    //魅力指数 (0,100)
    public short charm;
    //魅力标签 平平(0,20)，清秀(21,40)，亮色(41,60)，出众(61,80)，超凡(81,100) 根据魅力指数取值
    public string charmTag;
    //传播倾向
    public short rumorType;
    //偏好物品
    public ItemSort preferred;
    #endregion
    #region 财富财产情况
    //private持有资金 根据职务等级随机
    public int money;
    //贫困,拮据,小康,中产,富有,豪门资产等级 (0,1,2,3,4,5)，根据持有资金调取string值：根据持有金取值：0:＜5000 1:25000,2:125000,3:625000,4:3125000,5:15625000
    public string wealthRankTag;
    public short wealthRank;
    //npc的自营企业
    public SelfEmployedAd selfEmployed;
    //根据房产配置表选择
    public List<Estate> estate = new List<Estate>();
    //居住地址
    public string livingAdress;
    //isprivate持股票列表
    public List<StockTrade> stock = new();
    //持有自营股份的列表
    public List<EquityStructure> equityStructures = new List<EquityStructure>();
    //持股公司
    public List<short> holdingCompany = new List<short>();
    //载具列表
    public List<Vehicles> vehicles = new List<Vehicles>();
    //isprivate持有物品列表
    public Dictionary<int,Item> items=new();
    //债权债务列表 以id记录
    public List<long> debts=new();
    #endregion
    #region 人际关系
    //是否认识主角 在与主角社交前均为FALSE（可能在名声系统产生影响）
    public bool knowPlayer;
    //是否展示个人信息 在与主角社交前均为FALSE
    public bool privateInfoShow;
    //是否展示私密信息
    public bool secretInfoShow;
    //是否进行过密会 在与主角社交前均为FALSE
    public bool sexWithPlayer;
    //personal// 在读取后 用一张list来显示关系值与player和其他NPC的关系值列表，取值范围(-100,100）
    public Dictionary<short,NpcRelationships> relationships =new();
    //personal婚恋情况 已婚2/恋爱1/单身0
    public short loveStatus;
    //npc倾慕的异性对象的id
    public short AdmirationObject;
    //npc的社交目标靶对象
    public List<short>targetNpc = new();
    #endregion
    #region 秘闻(消息相关）

    #endregion
    #region 社会影响等级
    // 0-底层，1-平庶，2-小资，3-中产，4-要人，5-权贵阶级等级 (0,1,2,3,4,5)按持有汽车等级的最高级、持有房产等级的最高级、资产门槛最高级、职务等级4个项目的平均值产生，四舍五不入
    public short socialRank;
    public string socialRankTag;
    #endregion
    #region 立绘相关
    //场景名，人数，男性数，女性数，等级，img路径，/    /场景资源/所属单位_场景名/
    public string image;
    //立绘名
    public string imageName;
    #endregion
    #endregion
}