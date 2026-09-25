#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
构建自走棋 320x180 UI 稿：
  1) 读取 template.html
  2) 从模板中提取所有用到的字符，对「缝合像素字体」简体版做子集化并内嵌
  3) 把用到的像素 UI 元件 (assets/common/textures/tile_XXXX.png) 转 base64 内嵌
  4) 输出单文件 index.html（可离线打开，无需任何外部依赖）
"""
import base64, io, os, re, sys

HERE = os.path.dirname(os.path.abspath(__file__))
REPO = os.path.abspath(os.path.join(HERE, '..'))
ASSET_DIR = os.path.join(REPO, 'assets', 'common', 'textures')
FONT_DIR = '/tmp/fontsrc'          # 上一步下载并解压的字体
TEMPLATE = os.path.join(HERE, 'template.html')
OUT = os.path.join(HERE, 'index.html')

TILES = ['0010','0012','0020','0028','0035','0048','0049','0060','0148']
FONTS = {
    'F8':  os.path.join(FONT_DIR, 'd8',  'fusion-pixel-8px-proportional-zh_hans.ttf.woff2'),
    'F10': os.path.join(FONT_DIR, 'd10', 'fusion-pixel-10px-proportional-zh_hans.ttf.woff2'),
    'F12': os.path.join(FONT_DIR, 'd12', 'fusion-pixel-12px-proportional-zh_hans.ttf.woff2'),
}

# 额外补充一些备用字，防止后续微调文字时缺字
EXTRA = ('敌人我方上下左右前后大小总计伤害治疗护甲魔法抗性技能冷却范围选择取消确定是否'
         '开始结束第一二三四五六七八九十百千万胜负名次等级金币经验生命法力攻击速度暴击'
         '商店刷新升级锁定出售装备羁绊刺客法师护卫神枪龙族战士秘术游侠影刃霜铁卫龙裔'
         '玩家回合准备战斗阶段海妖霜焰木王 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ'
         'abcdefghijklmnopqrstuvwxyz:./%+-×·()[],!?\'"　')


def collect_chars(text):
    s = set(EXTRA)
    for ch in text:
        o = ord(ch)
        if o > 0x2000 or 0x20 <= o <= 0x7E:
            s.add(ch)
    return s


def subset_font(src, codepoints):
    from fontTools import subset
    from fontTools.ttLib import TTFont

    opts = subset.Options()
    opts.flavor = 'woff2'
    opts.layout_features = ['*']
    opts.notdef_outline = True
    opts.recalc_bounds = True
    opts.drop_tables = ['DSIG']
    font = subset.load_font(src, opts)
    ss = subset.Subsetter(options=opts)
    ss.populate(unicodes=codepoints)
    ss.subset(font)
    buf = io.BytesIO()
    subset.save_font(font, buf, opts)
    return buf.getvalue(), font


def main():
    html = open(TEMPLATE, encoding='utf-8').read()
    chars = collect_chars(html)
    codepoints = sorted(ord(c) for c in chars)
    print('subset chars:', len(chars))

    font_b64 = {}
    for name, path in FONTS.items():
        data, font = subset_font(path, codepoints)
        cmap = font.getBestCmap()
        missing = [c for c in chars if ord(c) not in cmap and ord(c) > 0x2000]
        if name == 'F8':
            print('missing glyphs:', ''.join(missing) if missing else '(none)')
        font_b64[name] = base64.b64encode(data).decode('ascii')
        print(f'  {name}: {len(data)/1024:.1f} KB  -> {len(font_b64[name])/1024:.1f} KB base64')

    # 元件图
    tiles_js = []
    for t in TILES:
        p = os.path.join(ASSET_DIR, f'tile_{t}.png')
        b = base64.b64encode(open(p, 'rb').read()).decode('ascii')
        tiles_js.append(f"  '{t}': 'data:image/png;base64,{b}'")
    tiles_literal = '{\n' + ',\n'.join(tiles_js) + '\n}'

    html = html.replace('__F8__', font_b64['F8'])
    html = html.replace('__F10__', font_b64['F10'])
    html = html.replace('__F12__', font_b64['F12'])
    html = html.replace('__TILES__', tiles_literal)

    # 剩余占位符检查
    left = re.findall(r'__[A-Z0-9_]+__', html)
    if left:
        print('!! 未替换占位符:', set(left)); sys.exit(1)

    open(OUT, 'w', encoding='utf-8').write(html)
    print('written:', OUT, f'({os.path.getsize(OUT)/1024:.1f} KB)')


if __name__ == '__main__':
    main()
