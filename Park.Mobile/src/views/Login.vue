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

      <!-- <el-link href="register" >注册</el-link> -->
    </div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import Cookies from "js-cookie"
export default Vue.extend({
  data: function() {
    return { username: "user17392", password: "1234" };
  },
  methods: {
    login() {
        Vue.axios
        .post("http://localhost:8520/User/Login", {
          UserName: this.username,
          Password: this.password
        })
        .then(response => {
          Cookies.set("userID",response.data.data.userID)
          Cookies.set("token",response.data.data.token)
          Cookies.set("username",response.data.data.carOwner.username)
          window.location.href="/";
          console.log(response.data);
        }).catch(r=>{
          console.log(r.Message);
        })
    },
    register(){
       Vue.axios
        .post("http://localhost:8520/User/Login", {
          UserName: this.username,
          Password: this.password
        })
        .then(response => {
          console.log(response.data.Message);
        }).catch(r=>{
          console.log(r.Message);
        })
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