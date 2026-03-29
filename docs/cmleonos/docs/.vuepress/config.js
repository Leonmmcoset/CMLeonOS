import { defaultTheme } from '@vuepress/theme-default'
import { defineUserConfig } from 'vuepress'
import { viteBundler } from '@vuepress/bundler-vite'

export default defineUserConfig({
  lang: 'zh-CN',

  title: 'CMLeonOS官方文档站',
  description: 'CMLeonOS是一个x86架构64位操作系统，它的目标是提供一个简单、可靠、安全的操作系统环境。',

  theme: defaultTheme({
    // logo: 'https://vuejs.press/images/hero.png',
    navbar: ['/', '/get-started', '/lua', '/commands', '/syscall', '/codeblocks'],
  }),



  bundler: viteBundler(),
})
