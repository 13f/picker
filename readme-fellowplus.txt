
【数据获取】http://data.fellowplus.com/js/controllers/project.js?_=1459572308
0、途径，对页面（http://data.fellowplus.com/#/project/K_M36KR_PROJECT:4871/intro）打开开发者工具--网络，即可看到数据调用链接。
1、网站：http://dataapi.fellowplus.com/project/website?_id=7967&_token=sCtcTUDKT%2B2IKN6r532jADl8jpCTvTouLvMkixXK1a0%3D&id=K_M36KR_PROJECT:4871
2、项目：http://dataapi.fellowplus.com/project?_id=7967&_token=sCtcTUDKT%2B2IKN6r532jADl8jpCTvTouLvMkixXK1a0%3D&id=K_M36KR_PROJECT:4871
3、融资：http://dataapi.fellowplus.com/project/invest?_id=7967&_token=sCtcTUDKT%2B2IKN6r532jADl8jpCTvTouLvMkixXK1a0%3D&id=K_M36KR_PROJECT:4871
4、新闻：http://dataapi.fellowplus.com/project/news?_id=7967&_token=sCtcTUDKT%2B2IKN6r532jADl8jpCTvTouLvMkixXK1a0%3D&id=K_M36KR_PROJECT:4871&limit=4
5、公司：http://dataapi.fellowplus.com/project/company?_id=7967&_token=sCtcTUDKT%2B2IKN6r532jADl8jpCTvTouLvMkixXK1a0%3D&id=K_M36KR_PROJECT:4871

【数据处理】
从列表中获取没有成功处理的ProjectPreview：
select * from FellowPlusProjectPreview where Id not in (select Id from FellowPlusProject)

【数据遗留】
没有成功处理的ProjectPreview：
K_COMPANY:远东买卖宝网络科技有限公司
K_M36KR_PROJECT:4895
K_M36KR_PROJECT:30706
K_M36KR_PROJECT:144392
K_M36KR_PROJECT:186289
K_YCPAI_PROJECT:48622
K_PEDAILY_PROJECT:畅游网
K_PEDAILY_PROJECT:东软集团
K_PEDAILY_PROJECT:同程旅游

【Log】
1、有两个ProjectID在获取company的时候会触发internal server error：
K_M36KR_PROJECT:186289
K_M36KR_PROJECT:4895
处理：已经在抓取逻辑中Skip。
2、有一个ProjectID在获取company之后，某个company的Name为null：
K_YCPAI_PROJECT:31030
处理：抓取正常执行，但是跳过了该公司。