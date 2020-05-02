<template>
  <div class="login">
    <h1>登录</h1>
    <div class="box">
      <el-input id="name" v-model="username" placeholder="请输入帐号">
        <template slot="prepend">帐号</template>
      </el-input>

      <br />
      <el-input id="password" v-model="password" type="password" placeholder="请输入密码">
        <template slot="prepend">密码</template>
      </el-input>

      <br />
      <br />

      <el-button id="login" v-on:click="login" style="width:100%" type="primary">登录</el-button>

      <br />
      <br />      
      <el-button id="login" v-on:click="register" style="width:100%">注册</el-button>
      <el-button id="test" v-on:click="test" style="width:100%">测试</el-button>

      <!-- <el-link href="register" >注册</el-link> -->
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import Cookies from "js-cookie"
const transport = Vue.axios.create({
  //withCredentials: true,
})
export default Vue.extend({
  data: function() {
    return { username: "user17392", password: "1234" };
  },
  methods: {
    login() {
        transport
        .post("http://localhost:8520/User/Login", {
          UserName: this.username,
          Password: this.password
        })
        .then(response => {
          console.log(response.data);
        }).catch(r=>{
          console.log(r.Message);
        })
    },
    register(){
       transport
        .post("http://localhost:8520/User/Login", {
          UserName: this.username,
          Password: this.password
        })
        .then(response => {
          console.log(response.data.Message);
        }).catch(r=>{
          console.log(r.Message);
        })
    },
    test(){
        transport
        .post("http://localhost:8520/User/Car", {
          Car: {LicensePlate:"浙B12345"},
          Type: "add"
        })
        .then(response => {
          console.log(response.data);
        }).catch(r=>{
          console.log(r.Message);
        })
    },
    withToken(obj: object){
      console.log(obj);
    }
  }
});
</script>
<style>
.box {
  width: 100%;
}
.box .el-row {
  width: 100%;
}
</style>